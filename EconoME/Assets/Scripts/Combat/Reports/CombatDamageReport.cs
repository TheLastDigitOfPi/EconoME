using System.Collections.Generic;
using System.Linq;



public struct CombatReportItem
{
    public float DamageDone;
    public float HealingDone;
    public float DamageMitagated;
    public bool KilledEntity;

    public CombatReportItem Add(CombatReportItem other)
    {
        DamageDone += other.DamageDone;
        HealingDone += other.HealingDone;
        DamageMitagated += other.DamageMitagated;
        KilledEntity = other.KilledEntity;
        return this;
    }
}

public class CombatDamageReport
{

    List<CombatReportItem> _combatReportItems = new();
    public EntityCombatController Defender { get; private set; }
    public CombatAttackInstance Attack { get; private set; }
    public bool KilledEntity { get; private set; }

    public bool Immune { get; private set; }

    public CombatDamageReport(CombatAttackInstance attack, EntityCombatController defender)
    { Attack = attack; Defender = defender; }
    public static CombatDamageReport ImmunityReport { get { return new CombatDamageReport(true); } }

    CombatDamageReport(bool immune) {Immune = immune;}

    public bool AddReportItem(CombatReportItem reportItem)
    {
        if(KilledEntity || Immune)
            return false;
        _combatReportItems.Add(reportItem);
        KilledEntity = reportItem.KilledEntity;
        return true;
    }

    public CombatReportItem GetFinalReport()
    {
        CombatReportItem finalReport = new();
        foreach (var report in _combatReportItems)
        {
            finalReport = finalReport.Add(report);
        }
        return finalReport;
    }

    List<CombatReportModifier> modifiers = new();
    public void AddReportModifier(CombatReportModifier modifier)
    {
        modifiers.Add(modifier);
    }
    public CombatEntityDefenses CurrentDefenses;
    public DefenseStatChangeInstance DefenseChanges;
    public void RecalculateDamage()
    {
        var flatChanges = modifiers.FindAll(m => m.Priority == CombatReportPriority.FlatStatChanges);
        foreach (var change in flatChanges)
        {
            change.ApplyModifier(this);
        }
        CurrentDefenses.CalculateBaseStatChange(DefenseChanges);
        CurrentDefenses.CalculateSecondStatChange(DefenseChanges);

        var otherModifiers = modifiers.FindAll(m => m.Priority != CombatReportPriority.FlatStatChanges).ToList();
        foreach (var item in modifiers)
        {
            item.ApplyModifier(this);
        }

    }




    public void CalculateInitalDamage()
    {
        var attack = Attack.Attack;
    }
}

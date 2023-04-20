using System;
using System.Collections.Generic;
public static class CombatCalculator
{
    /*
     Option 1, we define steps that each item that impacts our calculations must subscribe to

    Step 0: Base Defense Multiplication
     Ex: Start at 5 base defense
    Item adds 1.2x base defense 5*1.2 = 6
    6*1.3

    Step 1: Base Defense Addition
    Each item will recieve our base stats and tell us how much they are going to be adding to it
    
    Item adds +2 base defense = 2

    Total base defense = 8

    Step 2: Defense Multiplication
    Each item will recieve the total base stats after the change, they will then determine how much they are adding to the total

    Item adds 20% defense = 8 * 0.2 = 1.6
    Item adds 2 defense = 2

    8+1.6+2=11.6
     
     */



}

using System;
using System.Collections.Generic;

public class Rule
{
    public string antecedentA;
    public string antecedentB;
    public Type conseqentState;
    public Predicate compare;
    public enum Predicate
    { And, Or, nAnd}

    public Rule(string antecedentA, string antecedentB, Type consequentState, Predicate compare)
    {
        this.antecedentA = antecedentA;
        this.antecedentB = antecedentB;
        this.conseqentState = consequentState;
        this.compare = compare;
    }

    public Type CheckRule(Dictionary<string, bool> stats)
    {
        bool antecedentABool = stats[antecedentA];
        bool antecedentBBool = stats[antecedentB];

        switch(compare)
        {
            case Predicate.And:
               if(antecedentABool && antecedentBBool)
                {
                    return conseqentState;
                }
                else
                {
                    return null;
                }

            case Predicate.Or:
                if(antecedentABool || antecedentBBool)
                {
                    return conseqentState;
                }
                else
                {
                    return null;
                }

            case Predicate.nAnd:
                if(!antecedentABool && !antecedentBBool)
                {
                    return conseqentState;
                }
                else
                {
                    return null;
                }

            default:
                return null;
        }
    }
}

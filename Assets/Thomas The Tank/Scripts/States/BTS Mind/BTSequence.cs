using System.Collections.Generic;
public class BTSequence : BTBaseNode
{
    protected List<BTBaseNode> btNodes = new List<BTBaseNode>();

    public BTSequence(List<BTBaseNode> btNodes)
    {
        this.btNodes = btNodes;
    }

    public override BTNodesStates Evaluate()
    {
        bool failed = false;

        //Check if failed
        //loop through each btnodes element and evaluating it to check if a success or failure
        foreach(BTBaseNode btNode in btNodes)
        {
            //if failed break out
            if(failed)
            {
                break;
            }

            switch(btNode.Evaluate())
            {
                case BTNodesStates.FAILURE:
                    btNodeState = BTNodesStates.FAILURE;
                    failed = true;
                    break;

                case BTNodesStates.SUCCESS:
                    btNodeState = BTNodesStates.SUCCESS;
                    failed = false;
                    continue; //go to next iteration in loop

                default:
                    btNodeState = BTNodesStates.FAILURE;
                    failed = true;
                    break;
            }
        }

        return btNodeState;
    }

}
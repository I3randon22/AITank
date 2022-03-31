using System.Collections.Generic;

public class BTSelector : BTBaseNode
{
    protected List<BTBaseNode> btNodes = new List<BTBaseNode>();

    public BTSelector(List<BTBaseNode> btNodes)
    {
        this.btNodes = btNodes;
    }

    public override BTNodesStates Evaluate()
    {
        foreach(BTBaseNode btNode in btNodes)
        {
            switch(btNode.Evaluate())
            {
                case BTNodesStates.FAILURE:
                    continue; //go to next iteration
                case BTNodesStates.SUCCESS:
                    btNodeState = BTNodesStates.SUCCESS;
                    return btNodeState; //Stop and return success
                default:
                    continue;
            }
        }

        btNodeState = BTNodesStates.FAILURE;
        return btNodeState;
    }
}

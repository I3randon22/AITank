public class BTAction : BTBaseNode
{

    public delegate BTNodesStates ActionNodeFunction(); //use to have pointers to a function

    private ActionNodeFunction btAction;

    public BTAction(ActionNodeFunction btAction) //What to do when action node is success
    {
        this.btAction = btAction;
    }

    public override BTNodesStates Evaluate() //Check action for success or failure
    {
        switch(btAction())
        {
            case BTNodesStates.SUCCESS:
                btNodeState = BTNodesStates.SUCCESS;
                return btNodeState;

            case BTNodesStates.FAILURE:
                btNodeState = BTNodesStates.FAILURE;
                return btNodeState;

            default:
                btNodeState = BTNodesStates.FAILURE;
                return btNodeState;
        }
    }
}

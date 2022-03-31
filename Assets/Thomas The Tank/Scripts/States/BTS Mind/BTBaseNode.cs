public abstract class BTBaseNode
{
    protected BTNodesStates btNodeState;

    public BTNodesStates BTNodeState
    {
        get
        {
            return btNodeState;
        }
    }

    public abstract BTNodesStates Evaluate();
}
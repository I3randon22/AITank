using System;

public abstract class BaseState_ThomasTheTank_FSM
{
    public abstract Type StateUpdate(); //Run once per frame
    public abstract Type StateEnter(); //When something needs to be done when entering a state
    public abstract Type StateExit(); //When something needs to be done when exiting state
}

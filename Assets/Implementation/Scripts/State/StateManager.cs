using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;

namespace CrazyPawn.Implementation
{
    public class StateManager : IStateCompleter, IStateProvider
    {
        #region Private Fields

        private readonly List<State> AllStateValues = Enum.GetValues(typeof(State)).OfType<State>().ToList();

        private SignalBus SignalBus;

        #endregion
        
        #region Accessors

        public State State { get; set; }
        
        #endregion
        
        #region Constructors

        [Inject]
        public StateManager(SignalBus signalBus)
        {
            SignalBus = signalBus;
            SetNewState(State.Init);
        }
        
        #endregion
        
        #region Class Implementation

        public void CompleteState(State state)
        {
            if (state != State)
            {
                return;
            }
            if (state == AllStateValues.LastOrDefault()) {
                Debug.LogWarning("[STATE] Trying to switch to new state beyond all states. Abort");
                return;
            }
            var newStateIndex = (int)state + 1;
            if (newStateIndex >= AllStateValues.Count)
            {
                throw new IndexOutOfRangeException("New state index goes out of range.");
            }

            SetNewState((State)newStateIndex);
        }

        private void SetNewState(State newState)
        {
            State = newState;
            SignalBus.Fire(new StateChangedSignal(State));
        }
        
        #endregion
    }
}
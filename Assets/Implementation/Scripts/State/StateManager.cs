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

        private readonly List<State> _allStateValues = Enum.GetValues(typeof(State)).OfType<State>().ToList();

        private SignalBus _signalBus;

        private StateChangedSignal _stateChangedSignal;

        #endregion
        
        #region Accessors

        private StateChangedSignal StateChangedSignal => CommonUtils.GetCached(ref _stateChangedSignal, () => new StateChangedSignal(State.Init));
        
        public State State { get; set; }
        
        #endregion
        
        #region Constructors

        [Inject]
        public StateManager(SignalBus signalBus)
        {
            _signalBus = signalBus;
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
            if (state == _allStateValues.LastOrDefault()) {
                Debug.LogWarning("[STATE] Trying to switch to new state beyond all states. Abort");
                return;
            }
            var newStateIndex = (int)state + 1;
            if (newStateIndex >= _allStateValues.Count)
            {
                throw new IndexOutOfRangeException("New state index goes out of range.");
            }

            
            SetNewState((State)newStateIndex);
        }

        private void SetNewState(State newState)
        {
            State = newState;
            StateChangedSignal.UpdateState(State);
            _signalBus.Fire<IStateChangedSignal>(StateChangedSignal);
        }
        
        #endregion
    }
}
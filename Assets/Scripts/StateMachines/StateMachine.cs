using System;
using System.Collections.Generic;
using Unity.Assertions;

namespace DOTSTemplate.States
{
    public delegate T StateFactory<out T>();

    public interface IStateMachine<in TTrigger>
    {
        void Fire(TTrigger trigger);
    }
    
    public class StateMachine<TTrigger> : IStateMachine<TTrigger>
    {
        private readonly Dictionary<Type, IStateDefinition> states = 
            new Dictionary<Type, IStateDefinition>();
        private readonly Dictionary<(Type, TTrigger), IStateDefinition> transitions = 
            new Dictionary<(Type, TTrigger), IStateDefinition>();

        private (IStateDefinition Definition, IState State) activeState;

        public void Fire(TTrigger trigger)
        {
            var activeDefinition = activeState.Definition;

            var transitionDefinition = (activeDefinition?.StateType, trigger);
            
            Assert.IsTrue(transitions.ContainsKey(transitionDefinition), 
                $"Transition from state {activeState.State?.ToString() ?? "ROOT"} not found by trigger {trigger}");

            activeDefinition = transitions[transitionDefinition];
            
            activeState.State?.OnExit();
            activeState = (activeDefinition, activeDefinition?.CreateState());
            activeState.State?.OnEnter();
        }

        public void Stop()
        {
            activeState.State?.OnExit();
            activeState = default;
        }

        public void DefineState<TState>(StateFactory<TState> factory) where TState : IState
        {
            states[typeof(TState)] = new StateMachine<TTrigger>.StateDefinition<TState>(factory);
        }

        public void DefineTransition<TState1, TState2>(TTrigger trigger)
            where TState1 : IState
            where TState2 : IState
        {
            var type1 = typeof(TState1);
            Assert.IsTrue(states.ContainsKey(type1), $"State of type {type1} not defined");
            var type2 = typeof(TState2);
            Assert.IsTrue(states.ContainsKey(type2), $"State of type {type2} not defined");

            transitions[(type1, trigger)] = states[type2];
        }
        
        public void DefineStartTransition<TState>(TTrigger trigger)
            where TState : IState
        {
            var type1 = typeof(TState);
            Assert.IsTrue(states.ContainsKey(type1), $"State of type {type1} not defined");

            transitions[(null, trigger)] = states[type1];
        }
        
        private interface IStateDefinition
        {
            IState CreateState();
            Type StateType { get; }
        }
        
        private class StateDefinition<TState> : IStateDefinition where TState : IState
        {
            private readonly StateFactory<TState> factory;

            public StateDefinition(StateFactory<TState> factory)
            {
                this.factory = factory;
            }

            public IState CreateState()
            {
                return factory();
            }

            public Type StateType => typeof(TState);
        }
    }
}
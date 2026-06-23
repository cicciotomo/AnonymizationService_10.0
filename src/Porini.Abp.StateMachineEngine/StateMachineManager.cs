using Microsoft.Extensions.Logging;
using Porini.Abp.StateMachineEngine.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace Porini.Abp.StateMachineEngine
{
    public class StateMachineManager : ISingletonDependency
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IStateMachineRepository _stateMachineRepository;
        private readonly ILogger<StateMachineManager> _logger;
        private readonly List<StateMachine> _stateMachines;

        public StateMachineManager(IServiceProvider serviceProvider, IStateMachineRepository stateMachineRepository, ILogger<StateMachineManager> logger)
        {
            _serviceProvider = serviceProvider;
            _stateMachineRepository = stateMachineRepository;
            _logger = logger;
            _stateMachines = new List<StateMachine>();
        }

        public async Task TryRunNewStateMachineAsync(StateMachine stateMachine)
        {
            _logger.LogInformation("Starting StateMachine of type {stateMachineType} with Id {stateMachineId}", stateMachine.GetType(), stateMachine.Id);

            if (_stateMachines.Any(sm => sm.Id == stateMachine.Id))
            {
                _logger.LogWarning("Input state machine could not be handled because there is already another state machine running with the same Id {stateMachineId} of type {stateMachineType}", stateMachine.Id, stateMachine.GetType());
                return;
            }

            _stateMachines.Add(stateMachine);

            await stateMachine.CurrentState.TryRunAsync(_serviceProvider, stateMachine);

            stateMachine.SetStarted();
            await _stateMachineRepository.UpdateAsync(stateMachine);
        }

        internal async Task DispatchEventToStateMachineAsync(Guid stateMachineId, Event eventToDispatch)
        {
            _logger.LogInformation("Dispatching event {eventType} to State Machine {stateMachineId}", eventToDispatch.GetType().Name, stateMachineId);

            var stateMachine = _stateMachines.FirstOrDefault(s => s.Id == stateMachineId);

            if (stateMachine is not null)
            {
                var nextState = stateMachine.HandleEvent(eventToDispatch);
                await _stateMachineRepository.UpdateAsync(stateMachine);
                await nextState.TryRunAsync(_serviceProvider, stateMachine);
            }
            else
            {
                _logger.LogWarning("No state machine found with Id {stateMachineId}", stateMachineId);
            }
        }

        internal async Task DispatchExceptionToStateMachineAsync(Guid stateMachineId, Exception exception)
        {
            _logger.LogInformation("Dispatching exception {eventType} to State Machine {stateMachineId}", exception.GetType().Name, stateMachineId);

            var stateMachine = _stateMachines.FirstOrDefault(s => s.Id == stateMachineId);

            if (stateMachine is not null)
            {
                var failureState = stateMachine.HandleException(exception);
                await _stateMachineRepository.UpdateAsync(stateMachine);
                await failureState.TryRunAsync(_serviceProvider, stateMachine);
            }
            else
            {
                _logger.LogWarning("No state machine found with Id {stateMachineId}", stateMachineId);
            }
        }

        internal async Task InitializeStateMachineManagerAsync()
        {
            _logger.LogInformation("Initializing state machine manager..");

            var persistedRunningStateMachines = await _stateMachineRepository.GetRunningStateMachinesAsync();

            foreach (var stateMachine in persistedRunningStateMachines)
            {
                var currentStateMachine = _stateMachines.FirstOrDefault(s => s.Id == stateMachine.Id);

                if (currentStateMachine is null)
                {
                    stateMachine.RebuildCurrentState();
                    _stateMachines.Add(stateMachine);
                    await stateMachine.CurrentState?.TryRunAsync(_serviceProvider, stateMachine);
                }
            }

            _logger.LogInformation("State machine manager initialization completed successfully");
        }
    }
}

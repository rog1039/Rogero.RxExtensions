# Rogero.RxExtensions

This is a library that provides additional Rx operators.

## UniqueThrottle

This watches the input stream for unique inputs. When an input arrives, the operator makes note and schedules the firing of this signal in the future, delayed by a constant delay. This delay is specified when creating the UniqueThrottle. While we have a signal X waiting to be sent, any additional signals of X are ignored. If signal X arrives at time = t, with a delay of d, the signal X will be sent at time t+d. If a signal X arrives after t+d, say at time t2, it is queued until the time is t2+d.  

If a signal Y is received at any time, say time = t3, it has no effect on signal X. Signal Y will then be queued and sent out at t3+d.

The algorithm tracks each unique input independently.

### Basic Algorithm
```
Receive(input)
  IsMonitored(input) if true then return; else
  BeginMonitor(input)
    AddToMonitoredList(input)
    ScheduleSignal(input)
      RemoveFromMonitoredList(input
      SendSignal(input)
```

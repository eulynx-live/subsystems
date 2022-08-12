# EULYNX Live Subsystem Simulators

This repository contains .NET executables that simulate the behavior of the following EULYNX subsystems:

 - Light Signal (SCI-LS)
 - Point (SCI-P)
 - Train Detection System (SCI-TDS)
 
When paired with a *RaSTA Bridge*, they can be used to communicate with a EULYNX Digital Interlocking.

The SCI implementations are not complete, but implement only the most basic functionality.

Additional gRPC protocol specifications are in place for reading and manipulating the simulator state from within test scripts and train simulators.

Pull requests are welcome!

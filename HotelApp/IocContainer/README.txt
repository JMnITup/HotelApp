James Meyer - 2013-12-17

This solution is a prototype of a hand-rolled IOC container.  It is intended to allow for the highest level registration of concrete classes but the latest possible creation.  It relies on
and encourages interface-centric code, and is intended to move the responsibility of object creation and maintenance into a common location.

InterfaceResolver can be used as an instantiated instance and scattered through an object heirarchy, allowing each class to reference a container, 
or it can be used with a single static (or small number of assembly-level) instances.  The AssemblyIocContainer class is an example of a static instance, and could be created on a per-project basis.  
Finally, the global namespace could be used, and an assumption that all resolution used the global namespace 'AssemblyIocContainer' instance.
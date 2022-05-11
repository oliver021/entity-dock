using System;


namespace EntityDock.Lib.Auto
{

    /// <summary>
    /// Represent the model role type for usage by controller
    /// </summary>
    public enum ModelType
    {
        Readonly,
        Record,
        FullyFeatures
    }

    /// <summary>
    /// Simple unit for an entity that shloud be pushed in controller group
    /// </summary>
    public class UnitRoute
    { 
        public Type Model { get; set; }

        public ModelType ModelType { get; set; }

        public bool UseMappedModel { get; set; }
    }
}

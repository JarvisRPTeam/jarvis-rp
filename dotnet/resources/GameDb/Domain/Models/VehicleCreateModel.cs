namespace GameDb.Domain.Models {
    public class VehicleCreateModel
    {
        public string Model { get; set; }
        public string NumberPlate { get; set; }
        public float TankCapacity { get; set; }
        public float FuelConsumption { get; set; } // l/km
        public float Mileage { get; set; } // total km driven
        public PositionModel Position { get; set; }
        public VehicleColorModel Color { get; set; } 
    }
}
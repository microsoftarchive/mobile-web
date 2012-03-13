namespace MileageStats.Domain.Models
{
    public interface IVehicleStatistics
    {
        /// <summary>
        /// Gets the average cost to fill up per unit.
        /// </summary>
        double AverageFillupPrice { get; }

        /// <summary>
        /// Gets the average fuel efficiency (e.g. Miles/Gallon or Kilomter / Litre)
        /// </summary>
        double AverageFuelEfficiency { get; }

        /// <summary>
        /// Gets the average cost to drive per distance (e.g. $/Mile or €/Kilometer)
        /// </summary>
        double AverageCostToDrive { get; }

        /// <summary>
        /// Gets the average cost to drive per month (e.g. $/Month or €/Month) between the first entry and today.
        /// </summary>
        double AverageCostPerMonth { get; }

        /// <summary>
        /// Gets the highest odometer value recorded.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        int? Odometer { get; }

        /// <summary>
        /// Gets the total vehicle distance traveled for fillup entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        int TotalDistance { get; }

        /// <summary>
        /// Gets the total cost of all fillup entries, not including transaction fees.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        double TotalFuelCost { get; }

        /// <summary>
        /// Gets the total units consumed based on all fillup entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        double TotalUnits { get; }

        /// <summary>
        /// Gets the total cost of all fillup entries including transaction fees and service entries.
        /// </summary>
        /// <remarks>
        /// This is a calculated value and should not be set directly.
        /// </remarks>
        double TotalCost { get; }
    }
}
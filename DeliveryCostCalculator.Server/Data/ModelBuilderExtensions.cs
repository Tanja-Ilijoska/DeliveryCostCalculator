using DeliveryCostCalculator.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace DeliveryCostCalculator.Server.Data;
public static class ModelBuilderExtensions
{
    public static void Seed(this ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DeliveryService>().HasData(
            new DeliveryService()
            {
                Id = 1,
                Name = "Standard Delivery Service",
                Formula = ""
            },
            new DeliveryService()
            {
                Id = 2,
                Name = "Expedited Delivery Service",
                Formula = ""
            }
        );
        modelBuilder.Entity<DeliveryServiceProperties>().HasData(
            new DeliveryServiceProperties 
            { 
                Id = 1, 
                DeliveryServiceId = 1 , 
                Order = 1, Name = "Fuel Price",
                Operation = "+",
                Value = 100 
            },
            new DeliveryServiceProperties 
            { 
                Id = 2, 
                DeliveryServiceId = 1,
                Order = 2, 
                Name = "Carrier Rate",
                Operation = "+", 
                Value = 200 
            },
            new DeliveryServiceProperties 
            { 
                Id = 3, 
                DeliveryServiceId = 1, 
                Order = 3, Name = "Option",
                Operation = "+", 
                Value = 300 
            },

            new DeliveryServiceProperties
            { 
                Id = 4,
                DeliveryServiceId = 2,
                Order = 1,
                Name = "Fuel Price", 
                Operation = "+", 
                Value = 100 
            },
            new DeliveryServiceProperties
            { 
                Id = 5,
                DeliveryServiceId = 2,
                Order = 2,
                Name = "Carrier Rate",
                Operation = "+", 
                Value = 300 
            },
            new DeliveryServiceProperties
            { 
                Id = 6, 
                DeliveryServiceId = 2, 
                Order = 3,
                Name = "Option",
                Operation = "+", 
                Value = 400 
            }
        );

        modelBuilder.Entity<Country>().HasData(
           new Country()
           {
               Id = 1,
               Name = "Macedonia",
               CountryType = "2"
           },
           new Country()
           {
               Id = 2,
               Name = "Serbia",
               CountryType = "1",
               CostCorrectionPercentage = 3,
           },
           new Country()
           {
               Id = 3,
               Name = "Albania",
               CountryType = "1",
               CostCorrectionPercentage = 3,
           },
           new Country()
           {
               Id = 4,
               Name = "Kosovo",
               CountryType = "1",
               CostCorrectionPercentage = 3,
           },
           new Country()
           {
               Id = 5,
               Name = "Slovenia",
               CountryType = "0",
               CostCorrectionPercentage = 5,
           },
           new Country()
           {
               Id = 6,
               Name = "Italy",
               CountryType = "0",
               CostCorrectionPercentage = 5,
           },
           new Country()
           {
               Id = 7,
               Name = "France",
               CountryType = "0",
               CostCorrectionPercentage = 5,
           }
       );
    }
}
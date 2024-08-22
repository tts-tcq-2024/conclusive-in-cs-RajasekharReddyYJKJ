using System;
using System.Collections.Generic;
 
public class TypeWiseAlert
{
    public enum BreachType
    {
        NORMAL,
        TOO_LOW,
        TOO_HIGH
    }
 
    public enum CoolingType
    {
        PASSIVE_COOLING,
        HI_ACTIVE_COOLING,
        MED_ACTIVE_COOLING
    }
 
    public enum AlertTarget
    {
        TO_CONTROLLER,
        TO_EMAIL
    }
 
    public struct BatteryCharacter
    {
        public CoolingType coolingType;
        public string brand;
    }
 
    public interface IAlertSender
    {
        void Send(BreachType breachType);
    }
 
    public class ControllerAlert : IAlertSender
    {
        public void Send(BreachType breachType)
        {
            const ushort header = 0xfeed;
            Console.WriteLine($"{header} : {breachType}\n");
        }
    }
 
    public class EmailAlert : IAlertSender
    {
        private readonly string recepient = "a.b@c.com";
 
        public void Send(BreachType breachType)
        {
            if (breachType == BreachType.NORMAL)
            {
                return;
            }
 
            Console.WriteLine($"To: {recepient}\n");
            string message = breachType == BreachType.TOO_LOW
                ? "Hi, the temperature is too low\n"
                : "Hi, the temperature is too high\n";
 
            Console.WriteLine(message);
        }
    }
 
    public static BreachType InferBreach(double value, double lowerLimit, double upperLimit)
    {
        if (value < lowerLimit)
        {
            return BreachType.TOO_LOW;
        }
        if (value > upperLimit)
        {
            return BreachType.TOO_HIGH;
        }
        return BreachType.NORMAL;
    }
 
    public static BreachType ClassifyTemperatureBreach(CoolingType coolingType, double temperatureInC)
    {
        var limits = new Dictionary<CoolingType, (int lowerLimit, int upperLimit)>
        {
            { CoolingType.PASSIVE_COOLING, (0, 35) },
            { CoolingType.HI_ACTIVE_COOLING, (0, 45) },
            { CoolingType.MED_ACTIVE_COOLING, (0, 40) }
        };
 
        var (lowerLimit, upperLimit) = limits[coolingType];
        return InferBreach(temperatureInC, lowerLimit, upperLimit);
    }
 
    public static void CheckAndAlert(AlertTarget alertTarget, BatteryCharacter batteryChar, double temperatureInC)
    {
        BreachType breachType = ClassifyTemperatureBreach(batteryChar.coolingType, temperatureInC);
        IAlertSender alertSender = alertTarget switch
        {
            AlertTarget.TO_CONTROLLER => new ControllerAlert(),
            AlertTarget.TO_EMAIL => new EmailAlert(),
            _ => throw new ArgumentException("Invalid alert target")
        };
 
        alertSender.Send(breachType);
    }
}
 

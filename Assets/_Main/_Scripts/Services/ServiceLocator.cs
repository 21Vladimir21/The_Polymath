using System;
using System.Collections.Generic;
using _Main._Scripts.LetterPooLogic;
using UnityEngine;

namespace _Main._Scripts.Services
{
    public class ServiceLocator
    {
        public static ServiceLocator Instance => _instance ??= new ServiceLocator();

        private static ServiceLocator _instance;
        private static readonly Dictionary<Type, IService> Services = new();


        public static void ClearInstance()
        {
            _instance = null;
        }

        public bool TryAddService(IService service)
        {
            if (Services.TryAdd(service.GetType(), service))
                return true;
            Debug.Log($"Error, the service {service.GetType()} already exists!");
            return false;
        }
        

        public TService GetServiceByType<TService>() where TService : class, IService
        {
            if (Services.TryGetValue(typeof(TService), out var service))
                return service as TService;

            return null;
        }
    }
}
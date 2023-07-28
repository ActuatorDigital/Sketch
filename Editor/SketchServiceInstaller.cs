#if USE_FLUME
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AIR.Flume;
using UnityEngine;

namespace Actuator.Sketch
{
    [RequireComponent(typeof(FlumeServiceContainer))]
    public class SketchServiceInstaller : MonoBehaviour
    {
        private ActivatedDependency[] ActivateDependencies(SketchDependsOnAttribute[] dependsOnAttributes)
        {
            var implementations = new Dictionary<Type, object>();
            List<ActivatedDependency> dependencies = new List<ActivatedDependency>();
            foreach (SketchDependsOnAttribute dependsOnAttribute in dependsOnAttributes)
            {
                var serviceImplementation = dependsOnAttribute.ServiceImplementation;

                object dependency;
                if (implementations.ContainsKey(serviceImplementation))
                {
                    dependency = implementations[serviceImplementation];
                }
                else
                {
                    dependency = serviceImplementation.IsSubclassOf(typeof(MonoBehaviour))
                        ? gameObject.AddComponent(serviceImplementation)
                        : Activator.CreateInstance(serviceImplementation);
                    implementations.Add(serviceImplementation, dependency);
                }

                dependencies.Add(new ActivatedDependency
                {
                    Instance = dependency,
                    ServiceType = dependsOnAttribute.ServiceType,
                });
            }

            return dependencies.ToArray();
        }

        private void Awake()
        => gameObject
            .GetComponent<FlumeServiceContainer>()
            .OnContainerReady += InstallServices;

        private void InstallServices(FlumeServiceContainer container)
        {
            var dependencies = ResolveDependencies();
            foreach (var activatedDependency in dependencies)
            {
                var bindingFlags = BindingFlags.Public | BindingFlags.Instance;
                var registerMethod = container.GetType()
                    .GetMethods(bindingFlags)
                    .Where(m => m.Name == nameof(FlumeServiceContainer.Register))
                    .ElementAt(1)
                    .MakeGenericMethod(activatedDependency.ServiceType);
                registerMethod?.Invoke(container, new[] { activatedDependency.Instance });
            }
        }

        private ActivatedDependency[] ResolveDependencies()
        {
            List<ActivatedDependency> activatedDependencies = new List<ActivatedDependency>();

            var components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                var dependsOnAttributes = component.GetType()
                    .GetCustomAttributes(typeof(SketchDependsOnAttribute), true)
                    .Cast<SketchDependsOnAttribute>()
                    .ToList();

                var sketchDependencyOverrideAttributes = (SketchDependsOverrideAttribute[]) component.GetType()
                    .GetCustomAttributes(typeof(SketchDependsOverrideAttribute), true);
                foreach (var dependencyOverride in sketchDependencyOverrideAttributes)
                {
                    dependsOnAttributes.Remove(dependencyOverride);
                    var dependsOnToOverride = dependsOnAttributes
                        .FirstOrDefault(d => d.ServiceType == dependencyOverride.ServiceType);
                    dependsOnAttributes.Remove(dependsOnToOverride);
                    dependsOnAttributes.Add(dependencyOverride);
                }

                var sketchHasDependency = dependsOnAttributes.Any();
                if (sketchHasDependency)
                {
                    var sketchDependsOnArr = dependsOnAttributes.ToArray();
                    var dependencies = ActivateDependencies(sketchDependsOnArr);
                    activatedDependencies.AddRange(dependencies);
                }
            }

            return activatedDependencies.ToArray();
        }

        private class ActivatedDependency
        {
            public object Instance;
            public Type ServiceType;
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Common.Generics
{
    /// <summary>
    /// Service loader - design pattern (adopted from Java implementation) and applied generically
    /// Generic utility to load types that correspond to a type (class or interface) from a file path
    /// </summary>
    /// <remarks>
    /// This loader will try to load by Reflection all assemblies for the path and subpaths, even 
    /// Microsoft and third party assemblies. 
    /// 
    /// "The java.util.ServiceLoader class helps you find, load, and use service providers. It searches 
    /// for service providers on your application's class path or in your runtime environment's 
    /// extensions directory. It loads them and enables your application to use the provider's APIs. 
    /// If you add new providers to the class path or runtime extension directory, the ServiceLoader 
    /// class finds them. If your application knows the provider interface, it can find and use 
    /// different implementations of that interface. You can use the first loadable instance of the 
    /// interface or iterate through all the available interfaces."  
    /// from: https://docs.oracle.com/javase/tutorial/ext/basics/spi.html#the-serviceloader-class 
    /// </remarks>
    /// <typeparam name="T">the type to find and return</typeparam>
    public class GenericServiceLoader<T> : IServiceLoader<T> where T : class
    {
        #region Fields
        private List<T> _foundTypes;
        #endregion

        #region Properties

        /// <summary>
        /// List of services found (if any)
        /// </summary>
        public IList<T> FoundServices
        {
            get
            {
                _foundTypes = _foundTypes ?? new List<T>();
                return _foundTypes;
            }
        }
        #endregion

        #region Publics
        /// <summary>
        /// Load types (services) that are found in a file path
        /// </summary>
        /// <param name="folderPath">the top path to search</param>
        /// <remarks>
        /// The property FoundServices will contain all services (instantiated objects of type T)
        /// The method can throw exceptions in case of any issues when searching
        /// or loading the assemblies or the service instantiation.
        /// The caller is responsible for handling and reporting.
        /// </remarks>
        /// <exception cref="NotSupportedException"></exception>
        /// <exception cref="TargetInvocationException"></exception>
        /// <exception cref="MethodAccessException"></exception>
        /// <exception cref="MemberAccessException"></exception>
        /// <exception cref="MissingMethodException"></exception>
        /// <exception cref="TypeLoadException"></exception>
        /// <exception cref="System.Runtime.InteropServices.InvalidComObjectException"></exception>
        /// <exception cref="System.Runtime.InteropServices.COMException"></exception>
        public void LoadServicesFrom(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                SearchAssembliesForService(folderPath);
            }
        }
        #endregion

        #region Privates
        private void SearchAssembliesForService(string folder)
        {
            string fileFilter = "*.dll";

            // After loading an assembly there is no way to unload it unless
            // it was loaded into a separate domain which is not our case.
            // You become responsible, forever, for what you have loaded.
            string[] files = Directory.GetFiles(folder, fileFilter, SearchOption.TopDirectoryOnly);

            foreach (var file in files)
            {
                SearchAssembly(file);
            }
        }

        private void SearchAssembly(string file)
        {
            try
            {

                Assembly assembly = Assembly.LoadFile(file);
                var types = assembly.GetExportedTypes()
                    .Where(t => t.IsClass
                        && !t.IsAbstract
                        && t.IsDefined(typeof(DynamicServiceAttribute))
                        && typeof(T).IsAssignableFrom(t));

                foreach (Type type in types)
                {
                    T activatedObject;
                    if (ActivateType(type, out activatedObject))
                    {
                        FoundServices.Add(activatedObject);
                    }
                }
            }
            // Exceptions arising out of Assembly.LoadFile that can be ignored
            // Reflection loading can fail on assemblies, this is normal,
            // a lot of Microsoft and third party assemblies are made to prevent
            // reflection.
            // It is normal to fail on these protected assemblies.  Just be sure
            // that the target assemblies are not protected from reflection.
            // A failure here should not prevent continued searching of more Assemblies.
            catch (ArgumentException) { }
            catch (FileLoadException) { }
            catch (FileNotFoundException) { }
            catch (BadImageFormatException) { }
            catch (ReflectionTypeLoadException) { }
        }

        private bool ActivateType(Type type, out T instantiatedObject)
        {
            instantiatedObject = (T)Activator.CreateInstance(type);
            return true;
        }
        #endregion
    }
}

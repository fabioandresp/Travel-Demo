using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Travel.Model.Context;

namespace Travel.Model.Base
{

    /// <summary>
    /// Este Delegado es una representación genérica que permite la injección de funcionalidades en tiempo de ejecución para acceder fácilmente a entidades en función de su llave genéricam la cual es el ID que implementan todos los BaseModel
    /// </summary>
    /// <typeparam name="TSource">Es el genérico de la entidad esperada en la salida.</typeparam>
    /// <typeparam name="TKey">Es el genérico de la entidad consultada en la entrada; Siempre se usará el Id como llave referencial.</typeparam>
    /// <returns></returns>
    public delegate TEspected GetEntity<TEspected,TKey,TContext>(TKey source) 
        where TEspected : BaseModel 
        where TKey:BaseModel
        where TContext:TDBContext;

    /// <summary>
    /// Esta clase abstracta permitirá:
    /// 1. Inversión de dependencias de tipo entidad (+Liskov Principle en los caso que aplique usar la clase hija como padre).
    /// 2. Facilitará crear extensiones de las clases ya compiladas para añadir nuevas funcionalidades.
    /// </summary>
    public record BaseModel
    {
        /// <summary>
        /// Generador de GUIDS
        /// </summary>
        private static readonly Guid guid;
        static BaseModel()
        {
            guid = Guid.NewGuid();
        }


        /// <summary>
        /// Permite convertir un objeto "json" compuesto en un diccionario a una instancia de un BaseModel, dado que los records son readonly, este método permitirá hacer un cast a un BaseModel de objetos compropiedades similares.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <returns></returns>
        public T Factory<T>(IDictionary<string,object> instance)
        {
            return default;
        }

        /// <summary>
        /// Generar un nuevo GUID
        /// </summary>
        public static string NewGUID()  => Guid.NewGuid().ToString();

    }
}

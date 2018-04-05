using System;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR

using UnityEditor;

#endif

using UnityEngine;
using System.Text.RegularExpressions;
using System.ComponentModel;

/// <summary>
/// Author  :   Hwan Kim
/// </summary>
public static class Extension
{
    public static T GetRandomElement<T>(this IList<T> list)
    {
        // If there are no elements in the collection, return the default value of T
        if (list.Count == 0)
            return default(T);

        // Guids as well as the hash code for a guid will be unique and thus random
        int hashCode = Math.Abs(Guid.NewGuid().GetHashCode());
        return list[hashCode % list.Count];
    }

    public static TKey GetRandomKey<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        List<TKey> keys = Enumerable.ToList(dic.Keys);
        // If there are no elements in the collection, return the default value of T
        if (dic.Keys.Count == 0)
            return default(TKey);

        // Guids as well as the hash code for a guid will be unique and thus random
        int hashCode = Math.Abs(Guid.NewGuid().GetHashCode());
        return keys[hashCode % dic.Keys.Count];
    }

    public static TValue GetRandomValue<TKey, TValue>(this Dictionary<TKey, TValue> dic)
    {
        TKey key = dic.GetRandomKey<TKey, TValue>();
        return dic[key];
    }

    public static TKey NextKeyOf<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey item)
    {
        List<TKey> keys = Enumerable.ToList(dic.Keys);
        return keys.NextOf<TKey>(item);
    }

    public static T NextOf<T>(this IList<T> list, T item)
    {
        return list[(list.IndexOf(item) + 1) == list.Count ? 0 : (list.IndexOf(item) + 1)];
    }

    public static void AddNotContainOnly<T>(this IList<T> list, T item)
    {
        if (!list.Contains(item))
            list.Add(item);
    }

    /// <summary>
    /// Insert spaces between words on a camel-cased token
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string realWordString(this string text)
    {
        return Regex.Replace(text, "(\\B[A-Z])", " $1");
    }

    public static TAttribute GetAttribute<TAttribute>(this Enum value)
        where TAttribute : Attribute
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        return type.GetField(name)
            .GetCustomAttributes(false)
            .OfType<TAttribute>()
            .SingleOrDefault();
    }

    public static String GetDesc(this Enum value)
    {
        var description = GetAttribute<DescriptionAttribute>(value);
        return description != null ? description.Description : null;
    }

    static public Transform getChildTransfromByName(this Transform thisTran, string withName)
    {
        Transform[] ts = thisTran.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name == withName) return t.transform;
        return null;
    }

    static public Transform getChildTransfromByNameWithContain(this Transform thisTran, string withName)
    {
        Transform[] ts = thisTran.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in ts) if (t.gameObject.name.Contains(withName)) return t.transform;
        return null;
    }

    public static IEnumerable<T> getValues<T>()
    {
        return Enum.GetValues(typeof(T)).Cast<T>();
    }

    #region unity editor related extensions

#if UNITY_EDITOR

    public static Mesh clone(this Mesh mesh)
    {
        Mesh newmesh = new Mesh();
        newmesh.vertices = mesh.vertices;
        newmesh.triangles = mesh.triangles;
        newmesh.uv = mesh.uv;
        newmesh.normals = mesh.normals;
        newmesh.colors = mesh.colors;
        newmesh.tangents = mesh.tangents;
        AssetDatabase.CreateAsset(newmesh, AssetDatabase.GetAssetPath(mesh) + " copy.asset");
        return newmesh;
    }

    public static T SafeDestroy<T>(this T obj) where T : UnityEngine.Object
    {
        if (Application.isEditor)
            UnityEngine.Object.DestroyImmediate(obj);
        else
            UnityEngine.Object.Destroy(obj);

        return null;
    }

    public static T SafeDestroyGameObject<T>(this T component) where T : UnityEngine.Component
    {
        if (component != null)
            SafeDestroy(component.gameObject);
        return null;
    }

#endif

    #endregion unity editor related extensions
}
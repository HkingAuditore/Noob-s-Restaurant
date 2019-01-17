using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IContainable<T> //where T : Ingredient
{
    List<T> Contents { get; }

    /// <summary>
    /// 向此容器加入一项内容，需要实现数据列表(增添)和content位置的的更新
    /// </summary>
    /// <param name="content">加入的内容</param>
    void Add(T content);

    /// <summary>
    /// 向此容器加入一组内容，需要实现数据列表(增添)和content位置的的更新
    /// </summary>
    /// <param name="contents">加入的内容集合</param>
    void AddRange(List<T> contents);

    /// <summary>
    /// 从此容器中取出唯一确定的那一项，需要实现数据列表的更新(移除)，并调用目标容器的添加方法
    /// </summary>
    /// <param name="container"></param>
    T TakeTheOneTo(IContainable<T> container);

    /// <summary>
    /// 从此容器中取出某一项给定的内容，需要实现数据列表的更新(移除)，并调用目标容器的添加方法
    /// </summary>
    /// <param name="content"></param>
    /// <param name="container"></param>
    T TakeOneTo(T content, IContainable<T> container);

    /// <summary>
    /// 取出此容器中的所有内容，需要实现数据列表的更新(移除)，并调用目标容器的添加方法
    /// </summary>
    /// <returns></returns>
    List<T> TakeOutAllTo(IContainable<T> container);
}

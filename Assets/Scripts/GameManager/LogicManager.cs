﻿#if UNITY_EDITOR
#define DEBUG
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogicManager : MonoBehaviour {

	#region Fields
	/// <summary>
	/// ID队列的最大容量
	/// </summary>
	[SerializeField]
	[Header("ID队列的最大容量，参考场景内总Individual数量设置")]
	private int _MAX_IDQUEUE_SIZE = 100;

	/// <summary>
	/// ID队列，ID分配容器
	/// </summary>
	private static Queue<int> _IDQueue;

	/// <summary>
	/// 存活个体的ID列表字段
	/// </summary>
	private static List<Individual> _aliveIndividualList;

	/// <summary>
	/// ID-Individual查找表
	/// </summary>
	private static Dictionary<int, Individual> _IDToIndividualDictionary;
	#endregion

	#region Properties
	/// <summary>
	/// 存活个体的ID列表
	/// </summary>
	public static List<Individual> AliveIndividualList { get { return _aliveIndividualList; } }
	#endregion

	#region Public Methods
	/// <summary>
	/// Console输出Debug信息，需要DEBUG宏
	/// </summary>
	/// <param name="message">输出的信息</param>
	[System.Diagnostics.Conditional("DEBUG")]
	public static void Log(string message) {
		Debug.Log(message);
	}

	/// <summary>
	/// 在生成新的Individual时注册ID
	/// </summary>   
	/// <param name="ind">待注册的Individual</param>
	public static void RegisterIndividual(Individual ind) {
		_IDToIndividualDictionary.Add(_IDQueue.Dequeue(), ind);
		_aliveIndividualList.Add(ind);
		Log($"Individual { ind.ID } has successfully registered.");
	}

	/// <summary>
	/// 在生成英雄或者基地时注册ID
	/// </summary>   
	/// <param name="ind">待注册的Individual</param>
	/// <param name="ID">ID：英雄为0，基地为1</param>
	public static void RegisterIndividual(Individual ind, int ID) {
		_IDToIndividualDictionary.Add(ID, ind);
		_aliveIndividualList.Add(ind);
		Log($"Individual { ind.ID } has successfully registered.");
	}

	/// <summary>
	/// 在Individual死亡时注销Individual
	/// </summary>
	/// <param name="ind">死亡的Individual</param>
	public static void RemoveIndividual(Individual ind) {
		if (_IDToIndividualDictionary.ContainsKey(ind.ID)) {
			_IDToIndividualDictionary.Remove(ind.ID);
			_aliveIndividualList.Remove(ind);
			if (ind.ID != 0 && ind.ID != 1) {
				_IDQueue.Enqueue(ind.ID);
			}
		}
	}

	/// <summary>
	/// 通过ID获取Individual
	/// </summary>
	/// <param name="ID">待查找的ID</param>
	public static Individual GetIndividual(int ID) {
		if (_IDToIndividualDictionary.ContainsKey(ID)) {
			return _IDToIndividualDictionary[ID];
		}
		Log($"Individual { ID } is NOT found.");
		return null;
	}
	#endregion

	#region Private Methods
	bool IsPlayerDead() {
		Individual player = GetIndividual(0);
		if (player) {
			if (player.health <= 0) {
				return true;
			}
			return false;
		} else {
			//player不存在
			return true;
		}
	}

	bool IsBaseDestroyed() {
		Individual iBase = GetIndividual(1);
		if (iBase) {
			if (iBase.health <= 0) {
				return true;
			}
			return false;
		} else {
			//基地不存在
			return true;
		}
	}

	bool IsGameOver() {
		return IsPlayerDead() || IsBaseDestroyed();
	}
	#endregion

	#region Mono
	void Awake() {
		_IDQueue = new Queue<int>(_MAX_IDQUEUE_SIZE);
		_aliveIndividualList = new List<Individual>();
		_IDToIndividualDictionary = new Dictionary<int, Individual>();

		for (int id = 2; id < _MAX_IDQUEUE_SIZE; id++) {
			_IDQueue.Enqueue(id);
		}
	}

	void Start() {

	}

	void Update() {
		
	}
	#endregion
}

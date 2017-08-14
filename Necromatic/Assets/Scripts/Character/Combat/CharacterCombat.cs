﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UniRx;
using System;
namespace Necromatic.Char.Combat
{
    public enum Faction
    {
        Human,
        Undead
    }

    public class CharacterCombat : MonoBehaviour
    {
        [SerializeField]
        private WeaponBase _weapon;
        [SerializeField]
        private float _timeBeforeHit = 0.4f;
        [SerializeField]
        private Animator _animator;

        public Character CharacterScript
        {
            get
            {
                if(_characterScript == null)
                {
                    _characterScript = GetComponentInParent<Character>();
                }
                return _characterScript;
            }
        }

        private Character _characterScript;

        public Character CurrentTarget { get; private set; }
        public Faction _characterFaction;

        public ReactiveProperty<bool> Attacking = new ReactiveProperty<bool>();
        public WeaponBase Weapon => _weapon;

        public Character GetCurrentTarget()
        {
            return CurrentTarget;
        }

        public Character GetEnemy(float range)
        {
            var colliders = Physics.OverlapSphere(transform.position, range);
            var enemies = new List<Character>();
            foreach (var collider in colliders)
            {
                var combatScript = collider.gameObject.GetComponentInChildren<CharacterCombat>();
                if (combatScript != null && combatScript._characterFaction != _characterFaction)
                {
                    enemies.Add(collider.gameObject.GetComponent<Character>());
                }
            }
            if (enemies.Count > 0)
            {
                var minDistance = float.MaxValue;
                Character toReturn = null;
                foreach (var enemy in enemies)
                {
                    if (!enemy.IsDead.Value)
                    {
                        var dis = (enemy.transform.position - transform.position).magnitude;
                        if (dis < minDistance)
                        {
                            minDistance = dis;
                            toReturn = enemy;
                        }
                    }
                }
                return toReturn;
            }
            return null;
        }

        public void Attack(Character enemy) // for npcs
        {
            if (_weapon.CanAttack.Value)
            {
                _animator.SetBool("Attack", true);
                CurrentTarget = enemy;
                Attacking.Value = true;
                if (_weapon.IsMelee)
                {
                    Observable.Timer(TimeSpan.FromSeconds(_timeBeforeHit)).First().Subscribe(_ =>
                    {
                        _weapon.Attack(enemy, CharacterScript);
                    });
                }
                else
                {
                    _weapon.Attack(enemy, CharacterScript);
                }
                Observable.Timer(TimeSpan.FromSeconds(_weapon.Cooldown)).First().TakeUntilDestroy(this).Subscribe(_ =>
                {
                    Attacking.Value = false;
                    CurrentTarget = null;
                    if (gameObject.activeInHierarchy)
                    {
                        _animator.SetBool("Attack", false);
                    }
                });
            }
        }

        public void TryAttack() // for user
        {
            Character enemy = CurrentTarget ?? GetEnemy(_weapon.Range); // already have a target? otherwise fetch another
            if (enemy != null)
            {
                Attack(enemy);
            }
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Necromatic.Char.Combat;
namespace Necromatic.Char.NPC
{
    public class CharacterNPCCombat : MonoBehaviour
    {
        public Character CurrentTarget { get; private set; }
        public bool TargetOutOfRange { get; private set; }
        private CharacterCombat _combat;
        private bool _engageEnemy = true;
        [SerializeField] private float _detectionRange = 10f;

        public void Init(CharacterCombat combat)
        {
            _combat = combat;
        }

        public void ThinkCombat()
        {
            // look for enemies
            var enemy = CurrentTarget;
            if (enemy == null || enemy.IsDead.Value)
            {
                enemy = _combat.GetEnemy(_detectionRange);
            }

            if (enemy != null && _engageEnemy)
            {
                CurrentTarget = enemy;
                var dis = (enemy.transform.position - transform.position).magnitude;
                if (dis > _combat.Weapon.Range)
                {
                    TargetOutOfRange = true;
                }
                else
                {
                    TargetOutOfRange = false;
                    _combat.Attack(enemy);
                }
            }
        }
    }
}
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Necromatic.Char;
using Necromatic.Utility;
using UniRx;
using System;
namespace Necromatic.Char.NPC
{
    // no equivalent for player, because players have actual brains, hopefully
    public class CharacterNPC : MonoBehaviour
    {
        [SerializeField] private Character _characterScript;
        [SerializeField] private CharacterNPCMovement _npcMovement;
        [SerializeField] private CharacterNPCCombat _npcCombat;
        [SerializeField] private SpriteRenderer _selectionCircle;

        public Character Character => _characterScript;

        private Vector3 _destination = Vector3.zero;
        private bool _hasDestination = false;

        private const float _destinationMinDis = 0.05f;

        public void ToggleSelectionCircle(bool activated)
        {
            _selectionCircle.gameObject.SetActive(activated);
        }

        public void SetDestination(Vector3 destination)
        {
            _destination = destination;
            _hasDestination = true;
        }

        private TimeSpan _thinkRefresh = TimeSpan.FromSeconds(0.5f);
        void Awake()
        {
            Observable.Timer(TimeSpan.FromSeconds(0), _thinkRefresh)
                .TakeUntilDestroy(this)
                .Subscribe(_ => Think());
            _npcCombat.Init(_characterScript.Combat);
            _npcMovement.Init(_characterScript.Movement);
        }

        private void Think()
        {
            if (gameObject.activeInHierarchy)
            {
                _npcCombat.ThinkCombat();
            }
        }

        void FixedUpdate()
        {
            if (_hasDestination && Vector3Utils.DistanceGreater(transform.position, _destination, _destinationMinDis))
            {
                _npcMovement.NavigateTo(_destination);
            }
            else if(_hasDestination) // reached target
            {
                _hasDestination = false;
                _npcMovement.StopMoving();
            }
            else if (_npcCombat.CurrentTarget != null && _npcCombat.TargetOutOfRange) // combat ai found target, not close enough to attack
            {
                _npcMovement.NavigateTo(_npcCombat.CurrentTarget.transform.position);
            }
            else if(_npcCombat.CurrentTarget != null && !_npcCombat.TargetOutOfRange) // has target, stop moving
            {
                _npcMovement.StopMoving();
            }
        }
    }
}
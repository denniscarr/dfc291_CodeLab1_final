﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Explosion : MonoBehaviour {

    enum ExplosionState { Expanding, Fading };
    ExplosionState explosionState;

	[SerializeField] float explosionRadius;
    [SerializeField] float pushForce;
    [SerializeField] float expandDuration;
    [SerializeField] float fadeDuration;
    [SerializeField] int damageMin;
    [SerializeField] int damageMax;

    List<Collider> affectedObjects; // Contains references to all objects that have been collided with so that I don't hurt the same enemy multiple times.

    [SerializeField] GameObject explosionSphere;

    private GameManager gameManager;


    private void Start()
    {
        explosionState = ExplosionState.Expanding;

        explosionSphere.transform.DOScale(explosionRadius, expandDuration);
        affectedObjects = new List<Collider>();

        gameManager = FindObjectOfType<GameManager>();
    }


    private void Update()
    {
        if (explosionState == ExplosionState.Expanding)
        {
            if (explosionSphere.transform.localScale.x >= explosionRadius * 0.99f)
            {
                Debug.Log("Explosion reached full size.");
                explosionSphere.GetComponent<Renderer>().material.DOFade(0f, fadeDuration);
                explosionState = ExplosionState.Fading;
            }
        }

        else if (explosionState == ExplosionState.Fading)
        {
            if (explosionSphere.GetComponent<Renderer>().material.color.a <= 0.01f)
            {
                Debug.Log("Explosion deleting self.");
                Destroy(gameObject);
            }
        }
    }


    public void OnTriggerEnterChild(Collider collider)
    {
        if (explosionState != ExplosionState.Expanding) return;

        if (affectedObjects.Contains(collider)) return;
        else affectedObjects.Add(collider);

        if (collider.tag == "Player")
        {
            gameManager.PlayerHurt();
        }

        else if (collider.tag == "Enemy")
        {
            collider.GetComponent<Rigidbody>().AddExplosionForce(pushForce, transform.position, explosionSphere.transform.localScale.x);
            collider.GetComponent<Enemy>().HP -= Random.Range(damageMin, damageMax);
        }

        else if (LayerMask.LayerToName(collider.gameObject.layer).Contains("ShootableBullet"))
        {
            collider.transform.GetComponent<EnemyShot>().Detonate();
        }
    }
}

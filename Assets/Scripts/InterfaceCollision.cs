using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface InterfaceCollision
{
    void OnCollisionEnter(Collision collision);
    void OnTriggerEnter(Collider other);
    void StartSuccessSequence();
    void StartCrashSequence();
    void StartAcidSequence();
}

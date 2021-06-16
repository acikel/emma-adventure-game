using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LockResumePanel : ResumePanel
{

    private void OnEnable()
    {
        LockPanel.OnLockSolved += closeLockCanvas;
    }
    private void OnDisable()
    {
        LockPanel.OnLockSolved -= closeLockCanvas;
    }

    public void closeLockCanvas()
    {
        StartCoroutine(waitAndCloseLock());
        inventory.InteractionWithUIActive = false;

    }

    private IEnumerator waitAndCloseLock()
    {
        yield return new WaitForSeconds(0.2f);
        openCloseLockInventroyCanvas(false);
    }

    public override void onPointerAction()
    {
        LockPanel.InputCode = "";
    }
}

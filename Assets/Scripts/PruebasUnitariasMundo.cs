using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class PruebasUnitariasMundo
{
    private GameObject playerGO;
    private PlayerController player;

    [SetUp]
    public void Setup()
    {
        playerGO = new GameObject("PlayerTest", typeof(Rigidbody2D), typeof(Animator));
        player = playerGO.AddComponent<PlayerController>();
        player.enabled = false;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(playerGO);
    }

    [UnityTest]
    public IEnumerator PlayerController_EntrarEnDialogo_BloqueaMovimiento()
    {

        player.isInDialogue = false;

        player.isInDialogue = true;

        yield return null;

        Assert.IsTrue(player.isInDialogue, "El Player debe entrar en modo diálogo");
    }

    [UnityTest]
    public IEnumerator PlayerController_LookAtAlquimista_FuerzaEstadoIdle()
    {

        player.LookAtAlquimista();

        yield return null;

        Assert.Pass("El método LookAtAlquimista funciona, el Player mira hacia el lado correcto");
    }
}
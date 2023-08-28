using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// yield return new WaitForMillisecondFrames -> WaitFor

public class ExplosionJsonWriter : MonoBehaviour
{
#if UNITY_EDITOR
    private readonly Dictionary<string, List<ExplosionData>> dictionary = new();
    private readonly List<ExplosionData> list = new();

    private void WriteJsonEnemyBoss1(string enemyKey) {
        list.Clear();
        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_2);
        DeathExplosion(4900, new Effect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 1.225f), new MoveVector(5f, 180f)), 200);
        WaitFor(1500);

        DeathExplosion(2800, new Effect(ExplType.Normal_1, ExplAudioType.GroundSmall, Vector3.zero, 2.5f), new PairInt(450, 600));
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, Vector3.zero, 2.5f), new PairInt(150, 300), 2);

        WaitFor(3500);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_2); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.2f, 2.2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.2f, 2.2f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(2f, 0f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.2f, -2.2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.2f, -2.2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss1_Part(string enemyKey) {
        list.Clear();
        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_1);

        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-0.66f, 0f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.66f, 0f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-0.62f, 0f, 0.33f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.62f, 0f, 0.33f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(-0.69f, 0f, -0.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0.69f, 0f, -0.4f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss2(string enemyKey) {
        list.Clear();
        WaitFor(1500);

        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.GroundSmall, new Vector3(0f, 5.5f, 2f), 3f), new PairInt(450, 600));
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 0f), 5f), new PairInt(450, 600));
        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_1, new Vector3(0f, 5.5f, 3f), 3.5f), new PairInt(250, 350));
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 5f), 3.5f), new PairInt(250, 350));
        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.None, new Vector3(0f, 5.5f, 8f), 3f), new PairInt(250, 450));
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 8f), 3f), new PairInt(250, 450));

        WaitFor(3100);
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Huge_2, new Vector3(0f, 0f, 5f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(3f, 0f, 2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-3f, 0f, 2f));
        CreateExplosionEffect(ExplType.StarShape, ExplAudioType.None, new Vector3(4f, 0f, 5f));
        CreateExplosionEffect(ExplType.StarShape, ExplAudioType.None, new Vector3(-4f, 0f, 5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(3f, 0f, 8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-3f, 0f, 8f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss3_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(-1f, 0.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(1f, 0.4f));
        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_2, new Vector3(0f, 0.15f), 2.1f, new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)),
        new PairInt(350, 500));
        DeathExplosion(2000, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, new Vector3(0f, 0.15f), 1.6f, new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)),
        new PairInt(200, 400), 2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss3(string enemyKey) {
        list.Clear();
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.AirMedium_2, new Vector2(0f, -0.5f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 0.6f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.7f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.2f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.7f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.2f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));

        WaitFor(600);
        DeathExplosion(2800, new Effect(ExplType.Normal_1, ExplAudioType.GroundSmall, new Vector2(0f, 0.15f), 1.5f, new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)),
        new PairInt(500, 700), 2);
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, 0.15f), 1.5f, new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)),
        new PairInt(350, 500), 2);
        DeathExplosion(2800, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 0.15f), 1.5f, new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)),
        new PairInt(200, 400), 2);

        WaitFor(1000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, -0.8f), new PairFloat(0.7f, 0.7f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.25f, -0.4f), new PairFloat(0.7f, 0.7f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.25f, -0.4f), new PairFloat(0.7f, 0.7f), new PairFloat(0f, 360f));

        WaitFor(800);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(-0.45f, 0.5f), new PairFloat(1.2f, 1.2f), new PairFloat(80f, 100f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0.45f, 0.5f), new PairFloat(1.2f, 1.2f), new PairFloat(80f, 100f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.5f, 0.24f), new PairFloat(0.7f, 0.7f), new PairFloat(-10f, 10f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.5f, 0.24f), new PairFloat(0.7f, 0.7f), new PairFloat(-10f, 10f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.6f));
        
        WaitFor(1200);
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.Huge_2, new Vector2(0f, 0.64f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -0.12f), new PairFloat(1.8f, 1.8f), new PairFloat(-30f, -10f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -0.12f), new PairFloat(1.8f, 1.8f), new PairFloat(10f, 30f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.2f, 0.75f), new PairFloat(1.8f, 1.8f), new PairFloat(110f, 120f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.2f, 0.2f), new PairFloat(1.8f, 1.8f), new PairFloat(60f, 70f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.2f, 0.75f), new PairFloat(1.8f, 1.8f), new PairFloat(-120f, -110f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.2f, 0.2f), new PairFloat(1.8f, 1.8f), new PairFloat(-70f, -60f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0.2f, 0.18f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-0.2f, 0.18f), new PairFloat(0.75f, 1.25f), new PairFloat(0f, 360f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss4_NextPhase(string enemyKey) {
        list.Clear();

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 3f, -0.7f), 4.5f, new PairFloat(1.5f, 1.5f), new PairFloat(0f, 360f)),
        new PairInt(250, 350), 2);
        DeathExplosion(2000, new Effect(ExplType.Normal_3, ExplAudioType.GroundMedium, new Vector3(0f, 3f, -0.7f), 4.5f, new PairFloat(1.5f, 1.5f), new PairFloat(0f, 360f)),
        new PairInt(400, 600), 2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss4(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.None, ExplAudioType.Large);
        DeathExplosion(3200, new Effect(ExplType.Normal_3, ExplAudioType.AirMedium_1, new Vector3(0f, 3f, -0.7f), 4.5f),
        new PairInt(250, 350));
        DeathExplosion(3200, new Effect(ExplType.Simple_2, ExplAudioType.None, new Vector3(0f, 3f, -0.7f), 4.5f),
        new PairInt(250, 350), 2);
        DeathExplosion(3200, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, new Vector3(0f, 3f, -0.7f), 4.5f, new PairFloat(2f, 2f), new PairFloat(0f, 360f)),
        new PairInt(200, 600), 2);

        WaitFor(800);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.Large, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        WaitFor(800);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.Large, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        WaitFor(800);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.Large, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -0.5f), 4f);
        
        WaitFor(800);
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Huge_2, new Vector3(0f, 2f, 0f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-2f, 3f, -2f), new MoveVector(1.6f, -45f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(2f, 3f, -2f), new MoveVector(1.6f, 45f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(2f, 3f, 2f), new MoveVector(1.6f, 135f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-2f, 3f, 2f), new MoveVector(1.6f, -135f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-3.6f, 3f, -3.6f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(3.6f, 3f, -3.6f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(3.6f, 3f, 3.6f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-3.6f, 3f, 3.6f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-3f, 3f, 0f), new MoveVector(1.2f, -90f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, -3f), new MoveVector(1.2f, 0f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(3f, 3f, 0f), new MoveVector(1.2f, 90f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 3f, 3f), new MoveVector(1.2f, 180f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss5_NextPhase(string enemyKey) {
        list.Clear();
        
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.Large, new Vector2(0f, 0f), new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-3f, -3f), 0.8f, new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-3f, 3f), 0.8f, new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(3f, -3f), 0.8f, new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(3f, 3f), 0.8f, new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBoss5(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None);

        DeathExplosion(3200, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, Vector2.zero, 3f, new PairFloat(2f, 3.5f), new PairFloat(0f, 360f)),
        new PairInt(250, 350), 2);
        DeathExplosion(3200, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, Vector2.zero, 3f, new PairFloat(1f, 2f), new PairFloat(0f, 360f)),
        new PairInt(400, 600), 2);
        
        WaitFor(800);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, Vector2.zero, 3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, Vector2.zero, 3f);
        WaitFor(800);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, Vector2.zero, 3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, Vector2.zero, 3f);
        WaitFor(1200);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, Vector2.zero, 3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, Vector2.zero, 3f);

        WaitFor(1200);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_2); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, 0f), new PairFloat(3f, 3f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-4f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(4f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-2f, -3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-2f, 3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, -3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, 3.4f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBossFinal_NextPhase(string enemyKey) {
        list.Clear();
        
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, 0f), new PairFloat(1f, 1f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-1.5f, -1.5f), 0.5f, new PairFloat(1f, 1f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-1.5f, 1.5f), 0.5f, new PairFloat(1f, 1f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(1.5f, -1.5f), 0.5f, new PairFloat(1f, 1f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(1.5f, 1.5f), 0.5f, new PairFloat(1f, 1f), new PairFloat(0f, 360f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyBossFinal(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large);

        DeathExplosion(-1, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 1f, new PairFloat(5f, 5f), new PairFloat(160f, 200f)),
        new PairInt(200, 200));

        WaitFor(500);
        DeathExplosion(3600, new Effect(ExplType.Simple_2, ExplAudioType.AirSmall, Vector2.zero, 2f, new PairFloat(2f, 3.5f), new PairFloat(0f, 360f)),
        new PairInt(200, 350));
        DeathExplosion(3600, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 5f, new PairFloat(2f, 3.5f), new PairFloat(0f, 360f)),
        new PairInt(200, 350));
        DeathExplosion(3600, new Effect(ExplType.Normal_3, ExplAudioType.AirMedium_1, Vector2.zero, 2f, new PairFloat(2f, 3.5f), new PairFloat(0f, 360f)),
        new PairInt(200, 400));
        DeathExplosion(3600, new Effect(ExplType.Normal_3, ExplAudioType.None, Vector2.zero, 5f, new PairFloat(0f, 1f), new PairFloat(0f, 360f)),
        new PairInt(200, 400));
        DeathExplosion(3600, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, Vector2.zero, 4f, new PairFloat(1f, 2f), new PairFloat(0f, 360f)),
        new PairInt(100, 500));
        
        WaitFor(4000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_2); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-4f, 3f), new MoveVector(2f, 126.87f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(4f, 3f), new MoveVector(2f, -126.87f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 3f), new MoveVector(1.2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.5f, 2.4f), 0.5f, new PairFloat(0.8f, 0.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.5f, 2.4f), 0.5f, new PairFloat(0.8f, 0.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-3.5f, 0.4f), 0.5f, new PairFloat(0.8f, 0.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(3.5f, 0.4f), 0.5f, new PairFloat(0.8f, 0.8f), new PairFloat(0f, 360f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss1_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_1, new Vector2(-2f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, 0f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss1(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, Vector2.zero, 2.5f),
        new PairInt(200, 500), 2);
        DeathExplosion(1500, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_1, Vector2.zero, 2.5f),
        new PairInt(400, 700), 2);

        WaitFor(1600);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(3f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-3f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, 2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.5f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss2(string enemyKey) {
        list.Clear();

        DeathExplosion(1900, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 0f), 2f),
        new PairInt(200, 500), 2);
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.GroundMedium, new Vector3(0f, 2f, 0f), 2f),
        new PairInt(400, 700), 2);

        WaitFor(2000);
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Huge_1, new Vector3(-1f, 0f, 0f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, 0f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-1f, 0f, 1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, 1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-1f, 0f, -1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, -1.2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss3_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector3(0f, 3f, 2.8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(1.4f, 3f, 0.8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-1.4f, 3f, 0.8f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, -1.2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss3(string enemyKey) {
        list.Clear();

        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, new Vector3(0f, 3f, -5f), 2.5f),
        new PairInt(200, 400));
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.GroundSmall, new Vector3(0f, 3f, -5f), 2.5f),
        new PairInt(400, 700));
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, -1f), 2.5f),
        new PairInt(200, 400));
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, -1f), 2.5f),
        new PairInt(400, 700));
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, 3f), 2.5f),
        new PairInt(200, 400));
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, 3f), 2.5f),
        new PairInt(400, 700));

        WaitFor(2000);
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Huge_1, new Vector3(0f, 3f, 5f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(1.5f, 3f, 2f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-1.5f, 3f, 2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(0f, 3f, -1f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(1.5f, 3f, -4f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-1.5f, 3f, -4f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(0f, 3f, -7f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss4_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, -1.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.6f, 0.9f), 0.15f, new PairFloat(4.5f, 4.5f), new PairFloat(100f, 170f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.6f, 0.9f), 0.15f, new PairFloat(4.5f, 4.5f), new PairFloat(190f, 260f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.6f, -0.9f), 0.15f, new PairFloat(4.5f, 4.5f), new PairFloat(10f, 80f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.6f, -0.9f), 0.15f, new PairFloat(4.5f, 4.5f), new PairFloat(280f, 350f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss4(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Simple_2, ExplAudioType.AirSmall, new Vector2(0f, 0.3f), 2f),
        new PairInt(200, 400));
        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.GroundMedium, new Vector2(0f, 0.3f), 2f),
        new PairInt(500, 800));

        WaitFor(2000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large); // 최종 파괴
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(3f, 0f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(-3f, 0f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, 2f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, -1.5f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss4_Part(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.1f, 0f, 1.6f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0.1f, 0f, 0.6f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.1f, 0f, -0.4f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0.1f, 0f, -1.4f), 0.3f);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss5a(string enemyKey) {
        list.Clear();

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, -0.5f), 2.2f),
        new PairInt(200, 350));
        DeathExplosion(2000, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_2, new Vector2(0f, -0.5f), 2.2f),
        new PairInt(400, 700));

        WaitFor(2100);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-2f, -1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(2f, -1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-2f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(2f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -3f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss5a_Missile(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.Large, new Vector2(0f, -1f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 1f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss5b_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyMiddleBoss5b(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_2);

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 1.2f),
        new PairInt(200, 300));
        DeathExplosion(2000, new Effect(ExplType.Simple_2, ExplAudioType.AirMedium_1, Vector2.zero, 1.7f),
        new PairInt(150, 250));

        WaitFor(2100);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1, new Vector2(0f, 0f), new PairFloat(1.8f, 1.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.3f, 0f), new PairFloat(1.8f, 1.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.3f, 0f), new PairFloat(1.8f, 1.8f), new PairFloat(0f, 360f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, 1.4f), new PairFloat(1.8f, 1.8f), new PairFloat(0f, 360f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneLarge1(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_2);

        DeathExplosion(600, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, 3f), 2f),
        new PairInt(100, 150));
        DeathExplosion(600, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.8f), 1.5f),
        new PairInt(100, 150));
        DeathExplosion(600, new Effect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, 0f), 2f),
        new PairInt(100, 150));

        WaitFor(600);
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.Huge_1, new Vector2(0f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.5f, 3.5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.5f, 3.5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.8f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, -4.5f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-2f, -4.5f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneLarge1_NextPhase(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector3(-0.8f, 2f, 1.5f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector3(0.8f, 2f, 1.5f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneLarge2(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.5f, 2.5f));
        CreateExplosionEffect(ExplType.StarShape, ExplAudioType.None, new Vector2(0f, 2.5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.5f, 2.5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.5f, -3.8f));
        CreateExplosionEffect(ExplType.StarShape, ExplAudioType.None, new Vector2(0f, -3.8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.5f, -3.8f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneLarge3(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.8f, 0.24f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.8f, 0.24f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.2f, 0.36f), new PairFloat(1.4f, 2f), new PairFloat(45f, 45f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.2f, 0.36f), new PairFloat(1.4f, 2f), new PairFloat(-45f, -45f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(-10f, 10f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(110f, 130f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(230f, 250f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(-10f, 10f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(110f, 130f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.8f, 0.24f), new PairFloat(2f, 2.35f), new PairFloat(230f, 250f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneMedium1(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(2f, 0f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-2f, 0f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneMedium2(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(1.4f, 0.6f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-1.4f, 0.6f));
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, -1.3f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneMedium4(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1f, 1f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1f, 1f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.5f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyPlaneMedium5(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, 1.5f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -2f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyShipCarrier(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, 0f), 3f),
        new PairInt(100, 250));
        DeathExplosion(1500, new Effect(ExplType.StarShape, ExplAudioType.None, new Vector3(0f, 3f, 0f), 3f),
        new PairInt(100, 250));

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 3.8f), 4f),
        new PairInt(100, 250));
        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_2, new Vector3(0f, 2f, -3.8f), 4f),
        new PairInt(100, 250));

        WaitFor(1700);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(2f, 2f, 2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-2f, 2f, 2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(2f, 2f, 4f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-2f, 2f, 4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1, new Vector3(0f, 2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(2f, 2f, -2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-2f, 2f, -2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(2f, 2f, -4f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-2f, 2f, -4f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyShipMedium(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.GroundMedium);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(0f, 0f, -1.8f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyShipLarge(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.AirMedium_1);

        DeathExplosion(700, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 0f), 2f),
            new PairInt(100, 250), 2);

        WaitFor(700);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector3(0f, 2f, 1.7f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 2f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 2f, -2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyTankLarge1(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Large, new Vector3(0f, 0f, -0.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3.2f, 2.5f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-2f, 0f, 1.4f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(2f, 0f, 1.4f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-2f, 0f, -1.4f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(2f, 0f, -1.4f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyTankLarge2(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-1.5f, 0f, 1f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(1.5f, 0f, 1f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(-1.4f, 0f, -1.1f));
        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.None, new Vector3(1.4f, 0f, -1.1f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonEnemyTankLarge3(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_2, ExplAudioType.Large);
        CreateExplosionEffect(ExplType.Ground_1, ExplAudioType.None, new Vector3(-1.4f, 0f, -1.4f));
        CreateExplosionEffect(ExplType.Ground_1, ExplAudioType.None, new Vector3(1.4f, 0f, -1.4f));
        CreateExplosionEffect(ExplType.Ground_1, ExplAudioType.None, new Vector3(-1.4f, 0f, 1.4f));
        CreateExplosionEffect(ExplType.Ground_1, ExplAudioType.None, new Vector3(1.4f, 0f, 1.4f));

        dictionary[enemyKey] = new List<ExplosionData>(list);
    }
    


    public void GenerateJsonFile()
    {
        WriteJsonEnemyBoss1("EnemyBoss1");
        WriteJsonEnemyBoss1_Part("EnemyBoss1_Part");
        WriteJsonEnemyBoss2("EnemyBoss2");
        WriteJsonEnemyBoss3_NextPhase("EnemyBoss3_NextPhase");
        WriteJsonEnemyBoss3("EnemyBoss3");
        WriteJsonEnemyBoss4_NextPhase("EnemyBoss4_NextPhase");
        WriteJsonEnemyBoss4("EnemyBoss4");
        WriteJsonEnemyBoss5_NextPhase("EnemyBoss5_NextPhase");
        WriteJsonEnemyBoss5("EnemyBoss5");
        WriteJsonEnemyBossFinal("EnemyBossFinal");
        WriteJsonEnemyBossFinal_NextPhase("EnemyBossFinal_NextPhase");

        WriteJsonEnemyMiddleBoss1_NextPhase("EnemyMiddleBoss1_NextPhase");
        WriteJsonEnemyMiddleBoss1("EnemyMiddleBoss1");
        WriteJsonEnemyMiddleBoss2("EnemyMiddleBoss2");
        WriteJsonEnemyMiddleBoss3_NextPhase("EnemyMiddleBoss3_NextPhase");
        WriteJsonEnemyMiddleBoss3("EnemyMiddleBoss3");
        WriteJsonEnemyMiddleBoss4_NextPhase("EnemyMiddleBoss4_NextPhase");
        WriteJsonEnemyMiddleBoss4("EnemyMiddleBoss4");
        WriteJsonEnemyMiddleBoss4_Part("EnemyMiddleBoss4_Part");
        WriteJsonEnemyMiddleBoss5a("EnemyMiddleBoss5a");
        WriteJsonEnemyMiddleBoss5a_Missile("EnemyMiddleBoss5a_Missile");
        WriteJsonEnemyMiddleBoss5b_NextPhase("EnemyMiddleBoss5b_NextPhase");
        WriteJsonEnemyMiddleBoss5b("EnemyMiddleBoss5b");
        
        WriteJsonEnemyPlaneLarge1("EnemyPlaneLarge1");
        WriteJsonEnemyPlaneLarge1_NextPhase("EnemyPlaneLarge1_NextPhase");
        WriteJsonEnemyPlaneLarge2("EnemyPlaneLarge2");
        WriteJsonEnemyPlaneLarge3("EnemyPlaneLarge3");
        WriteJsonEnemyPlaneMedium1("EnemyPlaneMedium1");
        WriteJsonEnemyPlaneMedium2("EnemyPlaneMedium2");
        WriteJsonEnemyPlaneMedium4("EnemyPlaneMedium4");
        WriteJsonEnemyPlaneMedium5("EnemyPlaneMedium5");
        WriteJsonEnemyShipCarrier("EnemyShipCarrier");
        WriteJsonEnemyShipMedium("EnemyShipMedium");
        WriteJsonEnemyShipLarge("EnemyShipLarge");
        WriteJsonEnemyTankLarge1("EnemyTankLarge1");
        WriteJsonEnemyTankLarge2("EnemyTankLarge2");
        WriteJsonEnemyTankLarge3("EnemyTankLarge3");

        Utility.SaveDataFile(Application.streamingAssetsPath, "resources1.dat", dictionary);
    }


    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3 pos, float radius = 0f) {
        Vector3 explosion_pos = pos;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, radius);
        list.Add(new ExplosionData(effect, null, 0));
    }
    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3? pos = null, MoveVector? moveVector = null) {
        MoveVector explosion_moveVector = moveVector ?? new MoveVector(0f, 0f);
        Vector3 explosion_pos = pos ?? Vector3.zero;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, explosion_moveVector);
        list.Add(new ExplosionData(effect, null, 0));
    }
    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3 pos, PairFloat speed, PairFloat direction) {
        Vector3 explosion_pos = pos;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, speed, direction);
        list.Add(new ExplosionData(effect, null, 0));
    }
    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3 pos, float radius, PairFloat speed, PairFloat direction) {
        Vector3 explosion_pos = pos;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, radius, speed, direction);
        list.Add(new ExplosionData(effect, null, 0));
    }

    private void WaitFor(int wait) {
        list.Add(new ExplosionData(null, null, wait));
    }


    private void DeathExplosion(int duration, Effect effect, int timer_add, int number = 1) {
        list.Add(new ExplosionData(effect, new ExplosionCoroutine(duration, timer_add, number), 0));
    }
    private void DeathExplosion(int duration, Effect effect, PairInt timer_add, int number = 1) {
        list.Add(new ExplosionData(effect, new ExplosionCoroutine(duration, timer_add, number), 0));
    }
#endif
}

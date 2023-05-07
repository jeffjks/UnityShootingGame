using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

// yield return new WaitForMillisecondFrames -> WaitFor

public class ExplosionJsonWriter : MonoBehaviour
{
    Dictionary<string, List<ExplosionData>> dictionary = new Dictionary<string, List<ExplosionData>>();
    List<ExplosionData> list = new List<ExplosionData>();

    private void WriteJsonList1(string enemyKey) {
        list.Clear();
        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_2);
        DeathExplosion(4900, new Effect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 1.225f), new MoveVector(5f, 180f)), 200);
        WaitFor(1500);

        DeathExplosion(2800, new Effect(ExplType.Normal_1, ExplAudioType.GroundSmall, Vector3.zero, 2.5f), new int[] {450, 600});
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, Vector3.zero, 2.5f), new int[] {150, 300}, 2);

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

    private void WriteJsonList2(string enemyKey) {
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

    private void WriteJsonList3(string enemyKey) {
        list.Clear();
        WaitFor(1500);

        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.GroundSmall, new Vector3(0f, 5.5f, 2f), 3f), new int[] {450, 600});
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 0f), 5f), new int[] {450, 600});
        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_1, new Vector3(0f, 5.5f, 3f), 3.5f), new int[] {250, 350});
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 5f), 3.5f), new int[] {250, 350});
        DeathExplosion(2800, new Effect(ExplType.StarShape, ExplAudioType.None, new Vector3(0f, 5.5f, 8f), 3f), new int[] {250, 450});
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 5.5f, 8f), 3f), new int[] {250, 450});

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

    private void WriteJsonList4(string enemyKey) {
        list.Clear();

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_2, new Vector3(0f, 0.15f), 0.7f, new float[] {0.75f, 1.25f}, new float[] {0f, 360f}),
        new int[] {350, 500});
        DeathExplosion(2000, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, new Vector3(0f, 0.15f), 0.8f, new float[] {0.75f, 1.25f}, new float[] {0f, 360f}),
        new int[] {200, 400}, 2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList5(string enemyKey) {
        list.Clear();
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.AirMedium_2, new Vector2(0f, -0.5f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0f, 0.6f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.7f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.2f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.7f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.2f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});

        WaitFor(600);
        DeathExplosion(2800, new Effect(ExplType.Normal_1, ExplAudioType.GroundSmall, new Vector2(0f, 0.15f), 0.7f, new float[] {0.75f, 1.25f}, new float[] {0f, 360f}),
        new int[] {500, 700}, 2);
        DeathExplosion(2800, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, 0.15f), 0.7f, new float[] {0.75f, 1.25f}, new float[] {0f, 360f}),
        new int[] {350, 500}, 2);
        DeathExplosion(2800, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 0.15f), 0.7f, new float[] {0.75f, 1.25f}, new float[] {0f, 360f}),
        new int[] {200, 400}, 2);

        WaitFor(1000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, -0.8f), new float[] {0.7f, 0.7f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.25f, -0.4f), new float[] {0.7f, 0.7f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.25f, -0.4f), new float[] {0.7f, 0.7f}, new float[] {0f, 360f});

        WaitFor(800);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(-0.45f, 0.5f), new float[] {1.2f, 1.2f}, new float[] {80f, 100f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0.45f, 0.5f), new float[] {1.2f, 1.2f}, new float[] {80f, 100f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.5f, 0.24f), new float[] {0.7f, 0.7f}, new float[] {-10f, 10f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.5f, 0.24f), new float[] {0.7f, 0.7f}, new float[] {-10f, 10f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.4f, 0.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.4f, 0.6f));
        
        WaitFor(1200);
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.Huge_2, new Vector2(0f, 0.64f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f}); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -0.12f), new float[] {1.8f, 1.8f}, new float[] {-30f, -10f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -0.12f), new float[] {1.8f, 1.8f}, new float[] {10f, 30f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.2f, 0.75f), new float[] {1.8f, 1.8f}, new float[] {110f, 120f});
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(0.2f, 0.2f), new float[] {1.8f, 1.8f}, new float[] {60f, 70f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.2f, 0.75f), new float[] {1.8f, 1.8f}, new float[] {-120f, -110f});
        CreateExplosionEffect(ExplType.Normal_1, ExplAudioType.None, new Vector2(-0.2f, 0.2f), new float[] {1.8f, 1.8f}, new float[] {-70f, -60f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0.2f, 0.18f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-0.2f, 0.18f), new float[] {0.75f, 1.25f}, new float[] {0f, 360f});
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList6(string enemyKey) {
        list.Clear();

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 3f, -0.7f), 4.5f, new float[] {1.5f, 1.5f}, new float[] {0f, 360f}),
        new int[] {250, 350}, 2);
        DeathExplosion(2000, new Effect(ExplType.Normal_3, ExplAudioType.GroundMedium, new Vector3(0f, 3f, -0.7f), 4.5f, new float[] {1.5f, 1.5f}, new float[] {0f, 360f}),
        new int[] {400, 600}, 2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList7(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.None, ExplAudioType.Large);
        DeathExplosion(3200, new Effect(ExplType.Normal_3, ExplAudioType.AirMedium_1, new Vector3(0f, 3f, -0.7f), 4.5f),
        new int[] {250, 350});
        DeathExplosion(3200, new Effect(ExplType.Simple_2, ExplAudioType.None, new Vector3(0f, 3f, -0.7f), 4.5f),
        new int[] {250, 350}, 2);
        DeathExplosion(3200, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, new Vector3(0f, 3f, -0.7f), 4.5f, new float[] {2f, 2f}, new float[] {0f, 360f}),
        new int[] {200, 600}, 2);

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

    private void WriteJsonList8(string enemyKey) {
        list.Clear();
        
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.Large, new Vector2(0f, 0f), new float[] {3f, 3f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-3f, -3f), 0.8f, new float[] {3f, 3f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-3f, 3f), 0.8f, new float[] {3f, 3f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(3f, -3f), 0.8f, new float[] {3f, 3f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(3f, 3f), 0.8f, new float[] {3f, 3f}, new float[] {0f, 360f});
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList9(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None);

        DeathExplosion(3200, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, Vector2.zero, 3f, new float[] {2f, 3.5f}, new float[] {0f, 360f}),
        new int[] {250, 350}, 2);
        DeathExplosion(3200, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, Vector2.zero, 3f, new float[] {1f, 2f}, new float[] {0f, 360f}),
        new int[] {400, 600}, 2);
        
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
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, 0f), new float[] {3f, 3f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-4f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(4f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-2f, -3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-2f, 3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, -3.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, 3.4f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList10(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None);

        DeathExplosion(-1, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 1f, new float[] {5f, 5f}, new float[] {160f, 200f}),
        new int[] {200, 200});

        WaitFor(500);
        DeathExplosion(3600, new Effect(ExplType.Simple_2, ExplAudioType.AirSmall, Vector2.zero, 2f, new float[] {2f, 3.5f}, new float[] {0f, 360f}),
        new int[] {200, 350});
        DeathExplosion(3600, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 5f, new float[] {2f, 3.5f}, new float[] {0f, 360f}),
        new int[] {200, 350});
        DeathExplosion(3600, new Effect(ExplType.Normal_3, ExplAudioType.AirMedium_1, Vector2.zero, 2f, new float[] {2f, 3.5f}, new float[] {0f, 360f}),
        new int[] {200, 400});
        DeathExplosion(3600, new Effect(ExplType.Normal_3, ExplAudioType.None, Vector2.zero, 5f, new float[] {0f, 1f}, new float[] {0f, 360f}),
        new int[] {200, 400});
        DeathExplosion(3600, new Effect(ExplType.Normal_2, ExplAudioType.GroundSmall, Vector2.zero, 4f, new float[] {1f, 2f}, new float[] {0f, 360f}),
        new int[] {100, 500});
        
        WaitFor(4000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_2); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(-4f, 3f), new MoveVector(2f, 126.87f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(4f, 3f), new MoveVector(2f, -126.87f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 3f), new MoveVector(1.2f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.5f, 2.4f), 0.5f, new float[] {0.8f, 0.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.5f, 2.4f), 0.5f, new float[] {0.8f, 0.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-3.5f, 0.4f), 0.5f, new float[] {0.8f, 0.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(3.5f, 0.4f), 0.5f, new float[] {0.8f, 0.8f}, new float[] {0f, 360f});
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList11(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_1, new Vector2(-2f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(2f, 0f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList12(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, Vector2.zero, 2.5f),
        new int[] {200, 500}, 2);
        DeathExplosion(1500, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_1, Vector2.zero, 2.5f),
        new int[] {400, 700}, 2);

        WaitFor(1600);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(3f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-3f, 0f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, 2f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.5f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList13(string enemyKey) {
        list.Clear();

        DeathExplosion(1900, new Effect(ExplType.Normal_1, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 0f), 2f),
        new int[] {200, 500}, 2);
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.GroundMedium, new Vector3(0f, 2f, 0f), 2f),
        new int[] {400, 700}, 2);

        WaitFor(2000);
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.Huge_1, new Vector3(-1f, 0f, 0f)); // 최종 파괴
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, 0f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-1f, 0f, 1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, 1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(-1f, 0f, -1.2f));
        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.None, new Vector3(1f, 0f, -1.2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList14(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector3(0f, 3f, 2.8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(1.4f, 3f, 0.8f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(-1.4f, 3f, 0.8f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, -1.2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList15(string enemyKey) {
        list.Clear();

        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.AirSmall, new Vector3(0f, 3f, -5f), 2.5f),
        new int[] {200, 400});
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.GroundSmall, new Vector3(0f, 3f, -5f), 2.5f),
        new int[] {400, 700});
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, -1f), 2.5f),
        new int[] {200, 400});
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, -1f), 2.5f),
        new int[] {400, 700});
        DeathExplosion(1900, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, 3f), 2.5f),
        new int[] {200, 400});
        DeathExplosion(1900, new Effect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 3f, 3f), 2.5f),
        new int[] {400, 700});

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

    private void WriteJsonList16(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector2(0f, -1.4f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector2(0f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.6f, 0.9f), 0.15f, new float[] {4.5f, 4.5f}, new float[] {100f, 170f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.6f, 0.9f), 0.15f, new float[] {4.5f, 4.5f}, new float[] {190f, 260f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0.6f, -0.9f), 0.15f, new float[] {4.5f, 4.5f}, new float[] {10f, 80f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-0.6f, -0.9f), 0.15f, new float[] {4.5f, 4.5f}, new float[] {280f, 350f});
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList17(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Simple_2, ExplAudioType.AirSmall, new Vector2(0f, 0.3f), 2f),
        new int[] {200, 400});
        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.GroundMedium, new Vector2(0f, 0.3f), 2f),
        new int[] {500, 800});

        WaitFor(2000);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large); // 최종 파괴
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(3f, 0f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(-3f, 0f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, 2f));
        CreateExplosionEffect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, -1.5f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList18(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.1f, 0f, 1.6f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0.1f, 0f, 0.6f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0.1f, 0f, -0.4f), 0.3f);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0.1f, 0f, -1.4f), 0.3f);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList19(string enemyKey) {
        list.Clear();

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, -0.5f), 2.2f),
        new int[] {200, 350});
        DeathExplosion(2000, new Effect(ExplType.StarShape, ExplAudioType.AirMedium_2, new Vector2(0f, -0.5f), 2.2f),
        new int[] {400, 700});

        WaitFor(2100);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1); // 최종 파괴
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-2f, -1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(2f, -1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-2f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(2f, 1.6f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -3f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList20(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_2);
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList21(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.AirMedium_2);

        DeathExplosion(2000, new Effect(ExplType.Normal_2, ExplAudioType.None, Vector2.zero, 1.2f),
        new int[] {200, 300});
        DeathExplosion(2000, new Effect(ExplType.Simple_2, ExplAudioType.AirMedium_1, Vector2.zero, 1.7f),
        new int[] {150, 250});

        WaitFor(2100);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Huge_1, new Vector2(0f, 0f), new float[] {1.8f, 1.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(1.3f, 0f), new float[] {1.8f, 1.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(-1.3f, 0f), new float[] {1.8f, 1.8f}, new float[] {0f, 360f});
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, 1.4f), new float[] {1.8f, 1.8f}, new float[] {0f, 360f});
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }

    private void WriteJsonList22(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.None, ExplAudioType.AirMedium_2);

        DeathExplosion(600, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector2(0f, 3f), 2f),
        new int[] {100, 150});
        DeathExplosion(600, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector2(0f, -1.8f), 1.5f),
        new int[] {100, 150});
        DeathExplosion(600, new Effect(ExplType.Simple_2, ExplAudioType.None, new Vector2(0f, 0f), 2f),
        new int[] {100, 150});

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

    private void WriteJsonList23(string enemyKey) {
        list.Clear();

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 3f, 0f), 3f),
        new int[] {100, 250});
        DeathExplosion(1500, new Effect(ExplType.StarShape, ExplAudioType.None, new Vector3(0f, 3f, 0f), 3f),
        new int[] {100, 250});

        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 3.8f), 4f),
        new int[] {100, 250});
        DeathExplosion(1500, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_2, new Vector3(0f, 2f, -3.8f), 4f),
        new int[] {100, 250});

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

    private void WriteJsonList24(string enemyKey) {
        list.Clear();

        CreateExplosionEffect(ExplType.Ground_3, ExplAudioType.AirMedium_1);

        DeathExplosion(700, new Effect(ExplType.Normal_2, ExplAudioType.AirMedium_1, new Vector3(0f, 2f, 0f), 1.2f),
        new int[] {100, 250}, 2);

        WaitFor(700);
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.Large, new Vector3(0f, 2f, 1.7f));
        CreateExplosionEffect(ExplType.Normal_2, ExplAudioType.None, new Vector3(0f, 2f, 0f));
        CreateExplosionEffect(ExplType.Normal_3, ExplAudioType.None, new Vector3(0f, 2f, -2f));
        
        dictionary[enemyKey] = new List<ExplosionData>(list);
    }
    


    void Start()
    {
        WriteJsonList1("EnemyBoss1");
        WriteJsonList2("EnemyBoss1_Part");
        WriteJsonList3("EnemyBoss2");
        WriteJsonList4("EnemyBoss3_NextPhase");
        WriteJsonList5("EnemyBoss3");
        WriteJsonList6("EnemyBoss4_NextPhase");
        WriteJsonList7("EnemyBoss4");
        WriteJsonList8("EnemyBoss5_NextPhase");
        WriteJsonList9("EnemyBoss5");
        WriteJsonList10("EnemyBossFinal");

        WriteJsonList11("EnemyMiddleBoss1_NextPhase");
        WriteJsonList12("EnemyMiddleBoss1");
        WriteJsonList13("EnemyMiddleBoss2");
        WriteJsonList14("EnemyMiddleBoss3_NextPhase");
        WriteJsonList15("EnemyMiddleBoss3");
        WriteJsonList16("EnemyMiddleBoss4_NextPhase");
        WriteJsonList17("EnemyMiddleBoss4");
        WriteJsonList18("EnemyMiddleBoss4_Part");
        WriteJsonList19("EnemyMiddleBoss5a");
        WriteJsonList20("EnemyMiddleBoss5b_NextPhase");
        WriteJsonList21("EnemyMiddleBoss5b");
        
        WriteJsonList22("EnemyPlaneLarge1");
        WriteJsonList23("EnemyShipCarrier");
        WriteJsonList24("EnemyShipLarge");

        string str = JsonConvert.SerializeObject(dictionary, Formatting.None);
        //Debug.Log(str);

        FileStream fileStream = new FileStream(string.Format("{0}/{1}.json", Application.dataPath, "explosionData"), FileMode.Create);
        byte[] data = Encoding.UTF8.GetBytes(str);
        fileStream.Write(data, 0, data.Length);
        fileStream.Close();
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
    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3 pos, float[] speed, float[] direction) {
        Vector3 explosion_pos = pos;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, speed, direction);
        list.Add(new ExplosionData(effect, null, 0));
    }
    private void CreateExplosionEffect(ExplType ExplType, ExplAudioType audioType, Vector3 pos, float radius, float[] speed, float[] direction) {
        Vector3 explosion_pos = pos;

        Effect effect = new Effect(ExplType, audioType, explosion_pos, radius, speed, direction);
        list.Add(new ExplosionData(effect, null, 0));
    }

    private void WaitFor(int wait) {
        list.Add(new ExplosionData(null, null, wait));
    }


    private void DeathExplosion(int duration, Effect effect, int timer_add, int number = 1) {
        list.Add(new ExplosionData(effect, new Coroutine(duration, timer_add, number), 0));
    }
    private void DeathExplosion(int duration, Effect effect, int[] timer_add, int number = 1) {
        list.Add(new ExplosionData(effect, new Coroutine(duration, timer_add, number), 0));
    }
}

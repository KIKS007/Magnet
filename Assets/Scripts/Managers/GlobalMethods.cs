using UnityEngine;
using System.Collections;
using DG.Tweening;
using DarkTonic.MasterAudio;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GlobalMethods : Singleton<GlobalMethods>
{
    [Header("Layers")]
    public LayerMask gameplayLayer = (1 << 9) | (1 << 12);
    public LayerMask explosionMask = (1 << 9) | (1 << 12);

    [Header("Limits")]
    public Vector2 xLimits;
    public Vector2 zLimits;
    public float playerLimitReduction = 1.5f;

    [Header("DeathCount Text")]
    public GameObject deathTextPrefab;
    public Vector2 deathTextPositions;
    public float deathTextDuration;

    [Header("Spawn Feedback")]
    public GameObject[] spawnFeedbacks = new GameObject[3];

    [Header("Explosion")]
    public float explosionForce;
    public float explosionRadius;

    [HideInInspector]
    public float safeDuration = 1.5f;

    private int maxWhileLoop = 300;

    private const float checkSphereRadius = 4;

    private const float cubeYPosition = 3f;

    private const float defaultScaleDuration = 0.8f;

    private const float defaultDurationBetweenSpawn = 0.1f;

    private const float defaultfeedbackDuration = 0.5f;

    private const float defaultfeedbackWaitDuration = 0.5f;


    void Awake()
    {
        GlobalVariables.Instance.OnEndMode += () => StopAllCoroutines();
        GlobalVariables.Instance.OnMenu += () => StopAllCoroutines();
    }

    public void SetLimits()
    {
        xLimits.y = GameObject.FindGameObjectWithTag("CubesSpawnLimits").transform.GetChild(0).transform.position.x;
        zLimits.y = GameObject.FindGameObjectWithTag("CubesSpawnLimits").transform.GetChild(1).transform.position.z;

        xLimits.x = -xLimits.y;
        zLimits.x = -zLimits.y;
    }

    public void SpawnDeathText(PlayerName playerName, GameObject player, int count)
    {
        Vector3 position = new Vector3(player.transform.position.x, deathTextPositions.x, player.transform.position.z);

        GameObject text = Instantiate(deathTextPrefab, position, deathTextPrefab.transform.rotation);
        //text.transform.LookAt (GameObject.FindGameObjectWithTag ("MainCamera").transform);
        text.transform.GetChild(0).GetComponent<Outline>().effectColor = GlobalVariables.Instance.playersColors[(int)playerName];
        text.transform.GetChild(0).GetComponent<Text>().color = GlobalVariables.Instance.playersColors[(int)playerName];


        if (count == 0)
            text.transform.GetChild(0).GetComponent<Text>().text = "Dead!";
        else if (count != 1)
            text.transform.GetChild(0).GetComponent<Text>().text = count + " lives";
        else
            text.transform.GetChild(0).GetComponent<Text>().text = count + " life";
		

        text.transform.DOMoveY(deathTextPositions.y, deathTextDuration).SetEase(Ease.OutQuad).SetUpdate(false);
        text.transform.DOScale(0, 1f).SetEase(Ease.OutQuad).SetDelay(deathTextDuration * 0.9f).OnComplete(() => Destroy(text)).SetUpdate(false);
    }

    public void SpawnExistingPlayerRandomVoid(GameObject player, float timeBeforeSpawn = 0, bool waveAtSpawn = false)
    {
        StartCoroutine(SpawnExistingPlayerRandom(player, timeBeforeSpawn, waveAtSpawn));
    }

    IEnumerator SpawnExistingPlayerRandom(GameObject player, float timeBeforeSpawn = 0, bool waveAtSpawn = false)
    {
        Vector3 newPos = new Vector3();
        int loopCount = 0;

        player.SetActive(false);

        yield return new WaitForSeconds(timeBeforeSpawn);

        do
        {
            loopCount++;
            newPos = new Vector3(Random.Range(xLimits.x + playerLimitReduction, xLimits.y - playerLimitReduction), player.transform.position.y, Random.Range(zLimits.x + playerLimitReduction, zLimits.y - playerLimitReduction));
            yield return null;	
        }
        while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer) && loopCount < maxWhileLoop);

        player.layer = LayerMask.NameToLayer("Safe");
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        player.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        player.SetActive(true);
        player.transform.position = newPos;

        DOVirtual.DelayedCall(safeDuration, () =>
            {
                player.GetComponent<PlayersGameplay>().OnSafeEndVoid();
                player.layer = LayerMask.NameToLayer("Player");
            }).SetUpdate(false);

        //SpawnParticles (player);

        if (waveAtSpawn)
            player.GetComponent<PlayersFXAnimations>().WaveFX(true);
    }

    void SpawnParticles(GameObject player)
    {
        GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, player.transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;
	
        instantiatedParticles.transform.SetParent(GlobalVariables.Instance.ParticulesClonesParent);
        instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = GlobalVariables.Instance.playersColors[(int)player.gameObject.GetComponent<PlayersGameplay>().playerName];
    }

    public void SpawnPlayerDeadCubeVoid(PlayerName playerName, int controllerNumber, MovableScript script, float scaleDuration = defaultScaleDuration)
    {
        StartCoroutine(SpawnPlayerDeadCube(playerName, controllerNumber, script, scaleDuration));
    }

    IEnumerator SpawnPlayerDeadCube(PlayerName playerName, int controllerNumber, MovableScript script, float scaleDuration = defaultScaleDuration)
    {
        Vector3 newPos = new Vector3();
        int randomCube = Random.Range(0, GlobalVariables.Instance.deadCubesPrefabs.Length);
        int loopCount = 0;

        if (GlobalVariables.Instance.GameState == GameStateEnum.Playing)
        {
            yield return new WaitForSeconds(1f);
			
            do
            {
                loopCount++;
                newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
                yield return null;	
            }
            while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer) && loopCount < maxWhileLoop);			

            newPos.y = cubeYPosition;
            GameObject deadCube = Instantiate(GlobalVariables.Instance.deadCubesPrefabs[randomCube], newPos, GlobalVariables.Instance.deadCubesPrefabs[randomCube].transform.rotation, GameObject.FindGameObjectWithTag("MovableParent").transform) as GameObject;
			
            deadCube.GetComponent<PlayersDeadCube>().controllerNumber = controllerNumber;
            deadCube.GetComponent<PlayersDeadCube>().playerName = playerName;

            //
            var clonedScript = CopyComponent(script, deadCube);

            //Physic Material
            if (script.gameObject.GetComponent<Collider>().material != null)
                deadCube.GetComponent<Collider>().material = script.gameObject.GetComponent<Collider>().material;

            clonedScript.Awake();
            clonedScript.Start();
            clonedScript.OnEnable();

            Vector3 scale = deadCube.transform.lossyScale;
            deadCube.transform.localScale = Vector3.zero;
			
            deadCube.transform.DOScale(scale, scaleDuration).SetEase(Ease.OutElastic).SetUpdate(false);
            StartCoroutine(ChangeMovableTag(deadCube, deadCube.tag, scaleDuration));

            GameObject instantiatedParticles = Instantiate(GlobalVariables.Instance.PlayerSpawnParticles, deadCube.transform.position, GlobalVariables.Instance.PlayerSpawnParticles.transform.rotation) as GameObject;

            instantiatedParticles.transform.SetParent(GlobalVariables.Instance.ParticulesClonesParent);
            instantiatedParticles.GetComponent<ParticleSystemRenderer>().material.color = GlobalVariables.Instance.playersColors[(int)playerName];

            GameObject.FindGameObjectWithTag("MainCamera").GetComponent<DynamicCamera>().otherTargetsList.Add(deadCube);

            GlobalVariables.Instance.AllMovables.Add(deadCube);

            MasterAudio.PlaySound3DAtTransformAndForget(SoundsManager.Instance.cubeSpawnSound, deadCube.transform);

            SteamAchievements.Instance.UnlockAchievement(AchievementID.ACH_FIRST_REINCARNATION);
        }
    }

    public void RandomPositionMovablesVoid(GameObject[] allMovables = null, float scaleDuration = defaultScaleDuration, float sphereRadius = checkSphereRadius)
    {
        StartCoroutine(RandomPositionMovables(allMovables, scaleDuration, sphereRadius));
    }

    public IEnumerator RandomPositionMovables(GameObject[] allMovables = null, float scaleDuration = defaultScaleDuration, float sphereRadius = checkSphereRadius)
    {
        Vector3[] allScales = new Vector3[allMovables.Length];
        string[] allTags = new string[allMovables.Length];
        int loopCount = 0;
        float durationBetweenSpawn = (GlobalVariables.Instance.delayedStartupDuration - scaleDuration) / allMovables.Length;

        for (int i = 0; i < allMovables.Length; i++)
        {
            allMovables[i].SetActive(false);
            allScales[i] = allMovables[i].transform.lossyScale;
            allMovables[i].transform.localScale = new Vector3(0, 0, 0);
            allTags[i] = allMovables[i].tag;
        }

        yield return new WaitWhile(() => GlobalVariables.Instance.GameState != GameStateEnum.Playing);

        for (int i = 0; i < allMovables.Length; i++)
        {
            if (allMovables[i] != null)
                allMovables[i].tag = "Untagged";
			
            Vector3 newPos = new Vector3();

            do
            {
                loopCount++;
                newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
            }
            while(Physics.CheckSphere(newPos, sphereRadius, gameplayLayer) && loopCount < maxWhileLoop);

            yield return new WaitForSeconds(durationBetweenSpawn);

            newPos.y = cubeYPosition;

            if (allMovables[i] != null)
            {
                EnableGameObject(allMovables[i], newPos);
                ScaleGameObect(allMovables[i], allTags[i], allScales[i], scaleDuration);
            }
				
            yield return null;
        }
    }

    public void SpawnExistingMovableVoid(GameObject movable, Vector3 position, float scaleDuration = defaultScaleDuration)
    {
        StartCoroutine(SpawnExistingMovable(movable, position, scaleDuration));
    }

    IEnumerator SpawnExistingMovable(GameObject movable, Vector3 position, float scaleDuration = defaultScaleDuration)
    {
        Vector3 movableScale = movable.transform.lossyScale;
        movable.gameObject.SetActive(false);
        string tagTemp = movable.tag;
        movable.tag = "Untagged";

        yield return new WaitWhile(() => Physics.CheckSphere(position, checkSphereRadius, gameplayLayer));

        EnableGameObject(movable, position);
        ScaleGameObect(movable, tagTemp, movableScale, scaleDuration);
    }

    public void SpawnExistingMovableRandom(GameObject movable, float scaleDuration = defaultScaleDuration)
    {
        Vector3 movableScale = movable.transform.lossyScale;
        Vector3 newPos = new Vector3();
        string tagTemp = movable.tag;
        movable.tag = "Untagged";
        int loopCount = 0;

        do
        {
            loopCount++;
            newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
        }
        while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer) && loopCount < maxWhileLoop);

        newPos.y = cubeYPosition;

        EnableGameObject(movable, newPos);
        ScaleGameObect(movable, tagTemp, movableScale, scaleDuration);
    }

    public void SpawnExistingMovableRandom(Vector2 xLimits, Vector2 zLimits, GameObject movable, float scaleDuration = defaultScaleDuration)
    {
        Vector3 movableScale = movable.transform.lossyScale;
        Vector3 newPos = new Vector3();
        string tagTemp = movable.tag;
        movable.tag = "Untagged";
        int loopCount = 0;

        do
        {
            loopCount++;
            newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
        }
        while(Physics.CheckSphere(newPos, checkSphereRadius, gameplayLayer) && loopCount < maxWhileLoop);

        newPos.y = cubeYPosition;

        EnableGameObject(movable, newPos);
        ScaleGameObect(movable, tagTemp, movableScale, scaleDuration);
    }

    public void SpawnNewMovableRandomVoid(GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration, float checkSphere = checkSphereRadius)
    {
        StartCoroutine(SpawnNewMovableRandom(movable, delay, scaleDuration, checkSphere));
    }

    IEnumerator SpawnNewMovableRandom(GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration, float checkSphere = checkSphereRadius)
    {
        if (movable == null)
            movable = GlobalVariables.Instance.cubesPrefabs[Random.Range(0, GlobalVariables.Instance.cubesPrefabs.Length)];

        Vector3 movableScale = movable.transform.lossyScale;
        Vector3 newPos = new Vector3();
        string tagTemp = movable.tag;
        int loopCount = 0;

        GameObject clone = Instantiate(movable, newPos, Quaternion.Euler(Vector3.zero), movable.transform.parent) as GameObject;
        clone.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay);

        do
        {
            newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
            loopCount++;
        }
        while(Physics.CheckSphere(newPos, checkSphere, gameplayLayer) && loopCount < maxWhileLoop);

        newPos.y = cubeYPosition;
        clone.tag = "Untagged";

        EnableGameObject(clone, newPos);
        ScaleGameObect(clone, tagTemp, movableScale, scaleDuration);

        GlobalVariables.Instance.AllMovables.Add(clone);
    }

    public void SpawnNewMovableRandomFeedbackVoid(GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration, float checkSphere = checkSphereRadius, float feedbackDuration = defaultfeedbackDuration, float feedbackWaitDuration = defaultfeedbackWaitDuration)
    {
        StartCoroutine(SpawnNewMovableRandomFeedback(movable, delay, scaleDuration, checkSphere, feedbackDuration, feedbackWaitDuration));
    }

    IEnumerator SpawnNewMovableRandomFeedback(GameObject movable = null, float delay = 0, float scaleDuration = defaultScaleDuration, float checkSphere = checkSphereRadius, float feedbackDuration = defaultfeedbackDuration, float feedbackWaitDuration = defaultfeedbackWaitDuration)
    {
        if (movable == null)
            movable = GlobalVariables.Instance.cubesPrefabs[Random.Range(0, GlobalVariables.Instance.cubesPrefabs.Length)];

        Vector3 movableScale = movable.transform.lossyScale;
        Vector3 newPos = new Vector3();
        string tagTemp = movable.tag;
        int loopCount = 0;

        GameObject clone = Instantiate(movable, newPos, Quaternion.Euler(Vector3.zero), movable.transform.parent) as GameObject;
        clone.gameObject.SetActive(false);

        yield return new WaitForSeconds(delay);

        do
        {
            newPos = new Vector3(Random.Range(xLimits.x, xLimits.y), 1, Random.Range(zLimits.x, zLimits.y));
            loopCount++;
        }
        while(Physics.CheckSphere(newPos, checkSphere, gameplayLayer) && loopCount < maxWhileLoop);

        newPos.y = cubeYPosition;
        clone.tag = "Untagged";

        GameObject feedbackPrefab = null;

        if (movable.transform.localScale.x == 1.25f)
            feedbackPrefab = spawnFeedbacks[0];
        else if (movable.transform.localScale.x == 2f)
            feedbackPrefab = spawnFeedbacks[1];
        else if (movable.transform.localScale.x == 3f)
            feedbackPrefab = spawnFeedbacks[2];
		
        GameObject feedback = Instantiate(feedbackPrefab, new Vector3(newPos.x, 0.6f, newPos.z), feedbackPrefab.transform.rotation) as GameObject;
        Vector3 scale = feedback.transform.localScale;
        feedback.transform.localScale = Vector3.zero;

        SceneManager.MoveGameObjectToScene(feedback, SceneManager.GetSceneByName(GlobalVariables.Instance.CurrentModeLoaded.ToString()));

        feedback.transform.DOScale(scale, feedbackDuration).SetEase(Ease.OutElastic).SetUpdate(false);

        yield return new WaitForSeconds(feedbackDuration + feedbackWaitDuration * 0.5f);

        feedback.transform.DOScale(0, feedbackDuration).SetEase(Ease.OutQuad).SetUpdate(false).OnComplete(() => Destroy(feedback));

        yield return new WaitForSeconds(feedbackWaitDuration * 0.5f);

        EnableGameObject(clone, newPos);
        ScaleGameObect(clone, tagTemp, movableScale, scaleDuration);

        GlobalVariables.Instance.AllMovables.Add(clone);

        yield return 0;
    }

    void EnableGameObject(GameObject target, Vector3 position)
    {
        target.transform.position = position;

        target.GetComponent<Rigidbody>().velocity = Vector3.zero;
        target.GetComponent<Rigidbody>().angularVelocity = Vector3.zero;

        target.gameObject.SetActive(true);
    }

    public void ScaleGameObect(GameObject target, string tag, Vector3 scale, float scaleDuration)
    {
        target.transform.localScale = Vector3.zero;
        target.transform.DOScale(scale, scaleDuration).SetEase(Ease.OutElastic).SetUpdate(false);
        StartCoroutine(ChangeMovableTag(target, tag, scaleDuration));

        MasterAudio.PlaySound3DAtTransformAndForget(SoundsManager.Instance.cubeSpawnSound, target.transform);
    }

    public void Explosion(Vector3 explosionPosition, float explosionForce, float explosionRadius)
    {
        foreach (Collider other in Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask))
        {
            Vector3 repulseDirection = other.transform.position - explosionPosition;
            repulseDirection.Normalize();

            float explosionImpactZone = 1 - (Vector3.Distance(explosionPosition, other.transform.position) / explosionRadius);

            if (explosionImpactZone > 0)
            {
                if (other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddForce(repulseDirection * explosionImpactZone * explosionForce, ForceMode.Impulse);
            }
        }
    }

    public void Explosion(Vector3 explosionPosition)
    {
        foreach (Collider other in Physics.OverlapSphere(explosionPosition, explosionRadius, explosionMask))
        {
            Vector3 repulseDirection = other.transform.position - explosionPosition;
            repulseDirection.Normalize();

            float explosionImpactZone = 1 - (Vector3.Distance(explosionPosition, other.transform.position) / explosionRadius);

            if (explosionImpactZone > 0)
            {
                if (other.GetComponent<Rigidbody>() != null)
                    other.GetComponent<Rigidbody>().AddForce(repulseDirection * explosionImpactZone * explosionForce, ForceMode.Impulse);
            }
        }
    }

    public GameObject ExplosionFX(GameObject player, Vector3 position)
    {
        int playerNumber = (int)player.GetComponent<PlayersGameplay>().playerName;

        GameObject instance = Instantiate(GlobalVariables.Instance.explosionFX[playerNumber], position, GlobalVariables.Instance.explosionFX[playerNumber].transform.rotation) as GameObject;
        instance.transform.parent = GlobalVariables.Instance.ParticulesClonesParent.transform;

        MasterAudio.PlaySound3DAtVector3AndForget(SoundsManager.Instance.explosionSound, position);

        return instance;
    }

    IEnumerator ChangeMovableTag(GameObject movable, string tagTemp, float timeTween)
    {
        yield return new WaitForSeconds(0.3f * timeTween);

        if (movable != null)
            movable.tag = tagTemp;
    }

    public T CopyComponent<T>(T original, GameObject destination) where T : Component
    {
        System.Type type = original.GetType();
        Component copy = destination.AddComponent(type);
        System.Reflection.FieldInfo[] fields = type.GetFields();
        foreach (System.Reflection.FieldInfo field in fields)
        {
            field.SetValue(copy, field.GetValue(original));
        }
        return copy as T;
    }

    public void AddEventTriggerEntry(GameObject g, EventTriggerType type, System.Action action)
    {
        if (g.GetComponent<EventTrigger>() == null)
        {
            Debug.LogWarning("No Event Trigger Component : " + g.name);
            return;
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener((arg0) => action());

        g.GetComponent<EventTrigger>().triggers.Add(entry);
    }

    public void ReplaceInText(Text textComponent, string newValue, string oldValue = "$")
    {
        if (textComponent.text.Contains(oldValue))
            textComponent.text = textComponent.text.Replace(oldValue, newValue);
        else
            textComponent.text = newValue;
    }
}

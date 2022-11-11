using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class Weapon : Equipment
{
    // TECHNICALLY redundant for beam weapons, but we can just treat it as infinite velicity and say hey ho

    public enum WeaponType
    {
        projectile,
        beam,
        Special

    }

    public enum DamageType
    {
        kinetic,
        energy,
        thermal,
        ramming,
        shield

    }
    public bool switchable = false;
    public bool weaponEnergyIncreasesMuzzleVelocity = true;
    public bool weaponEnergyIncreasesDamage = true;
    public bool extraBonusToReloadWeaponEnergy = false;

    [HideInInspector]
    public float range = 1000f;

    public WeaponType weaponType = WeaponType.projectile;

    public AnimationCurve damageRampupMultiplierCurve = AnimationCurve.Linear(0f, 1f, 1f, 1f);
    public float farthestRampupThreshold = 2000f;
    public float closestRampupThreshold = 200f;

    [Header("UI Stuff")]
    

    public Sprite lockOnIcon;
    public Sprite crossHair;


    public Sprite shellIcon;

    
    public DamageType damageType = DamageType.kinetic;
    
    bool setMaterial = false;

    [Header("Generic Weapon Stats")]
    [SerializeField]
    protected int salvoSize = 1;
    protected int currentSalvo=0;
    protected float damageMultiplier = 1f;

    protected float reloadSpeedMultiplier = 1f;
    [Range(0.0f, 1.0f)]

   
    public float fireVolume = 0.4f;
    public float muzzleVelocity = 100f;

    protected float muzzleVelocityBase = 100f;
    public bool selected = false;
    public bool highlighted = false;

    public bool autoControlled = false;

    public bool initiallyAutoControlled = true;

    public bool allowAutoTarget = true;
    public string hudResource = "WeaponUIStandardised";
    protected GameObject hud;

    protected WeaponUi weaponUiInterface;

    GameObject mainHud;
    
    //When the mouse hovers over the GameObject, it turns to this color (red)
    [Header("MouseOver")]
    public Material mouseOverMaterialControlled;
    public Material mouseOverMaterialFriendly;

    //This stores the GameObject’s original color
    Color m_OriginalColor;

    //Get the GameObject’s mesh renderer to access the GameObject’s material and color
    MeshRenderer m_Renderer;

    List<Material> originalMats = new List<Material>();

    CameraLookController mainCamControl;
    CameraInteractionController mainCamInteraction;
    bool controllable = false;
    bool currentlyControlled = false; 

    public float calculateDamageRampup(float currentDistance){
        float damageRampupMultiplier = 1f;
        if(currentDistance < farthestRampupThreshold){
            // calculate value
            if(currentDistance < closestRampupThreshold){
                damageRampupMultiplier = damageRampupMultiplierCurve.Evaluate(0f);
            }
            else{
                float fraction = (currentDistance - closestRampupThreshold)/(farthestRampupThreshold-closestRampupThreshold);
                damageRampupMultiplier = damageRampupMultiplierCurve.Evaluate(fraction);
            }
           
        }
        return damageRampupMultiplier;

    }

    public bool getIsCurrentlyControlled(){
        return currentlyControlled;
    }
    public WeaponUi getGuiInterface(){
        return weaponUiInterface;
    }
    protected void reloadShells(int amount){
        currentSalvo += amount;
        if(currentSalvo > salvoSize) currentSalvo = salvoSize;
        weaponUiInterface.displayCurrentShells(currentSalvo, salvoSize);
    }
    protected int getCurrentSalvo(){
        return currentSalvo;
    }
    protected void decrementSalvo(){
        currentSalvo -=1;
        if(currentSalvo < 0) currentSalvo = 0;
        weaponUiInterface.displayCurrentShells(currentSalvo, salvoSize);
    }
    

    //returns current clip and clipSize
    public List<int> getSalvoInfo(){
        List<int> salvoInfo = new List<int>();
        salvoInfo.Add(currentSalvo);
        salvoInfo.Add(salvoSize);
        return salvoInfo;
    }
    
    public void setDamageMultiplier(float set){
        if(weaponEnergyIncreasesDamage) damageMultiplier = set;
        if(weaponEnergyIncreasesMuzzleVelocity){
            muzzleVelocity = muzzleVelocityBase * set;
            if(muzzleVelocity < muzzleVelocityBase) muzzleVelocity = muzzleVelocityBase;
        } 
    }
    public void setReloadTimeMultiplier(float set){
        reloadSpeedMultiplier = set;
        if(extraBonusToReloadWeaponEnergy){
            if(set > 1){
                set = ((set-1)*3)+set;
                reloadSpeedMultiplier = set;
            }
        }
    }

    public void toggleAutoControl(){
        autoControlled = !autoControlled;
    }
    public virtual bool canFire(){
        return true;
    }
    void weaponHud(){
        // get the current hud, disable it.
        mainHud.SetActive(false);
        // Enable custom weapon hud
       if(hud!=null) hud.SetActive(true);

    }
    void weaponCooldownHud(){

    }
    protected void weaponCooldownFreelookHud(float remainingTime, float maxTime ){
        if(GetComponent<WeaponBarOverlay>() != null){
            GetComponent<WeaponBarOverlay>().setRemainingTime(remainingTime, maxTime);
        }
    }

    void restoreHud(){
        // disable custom weapon hud
        // Enable ship hud
        if(hud!=null)hud.SetActive(false);
        mainCamControl.setDirectControlBool(true);
        mainHud.SetActive(true);
        mainCamInteraction.enabled = true;
    }

    public void selectWeapon(){
      //  if(selected){
            restoreHud();
            weaponHud();
            mainCamControl.setDirectControlBool(false);
            mainCamInteraction.enabled = false;
            selected = true;
            GetComponentInParent<shipWeaponList>().refreshList();
            GetComponentInParent<shipWeaponList>().calculateCurrentWeaponIndex();
            if(GetComponent<WeaponUserController>() != null)GetComponent<WeaponUserController>().enableDirectWeaponControl();
       // }
    }
    public void selectWeapon(Vector3 prevAimPos){
        restoreHud();
        weaponHud();
        if(GetComponent<WeaponUserController>() != null)GetComponent<WeaponUserController>().StartCoroutine("pauseOp");
        if(GetComponent<WeaponUserController>() != null)GetComponent<WeaponUserController>().forceCamLookat(prevAimPos);
        mainCamControl.setDirectControlBool(false);
        mainCamInteraction.enabled = false;
        selected = true;
        GetComponentInParent<shipWeaponList>().refreshList();
        GetComponentInParent<shipWeaponList>().calculateCurrentWeaponIndex();
        if(GetComponent<WeaponUserController>() != null)GetComponent<WeaponUserController>().enableDirectWeaponControl();
        
        setLookDir(prevAimPos, true);
    }
    public void semiDeselectWeapon(){
        if(hud!=null)hud.SetActive(false);
        selected = false;
        
    }
    public void deselectWeapon(){
        selected = false;
        restoreHud();
        if(GetComponent<WeaponUserController>() != null)GetComponent<WeaponUserController>().restoreFreeCam();
    }

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    protected void Awake()
    {
        hud =  Instantiate(Resources.Load(hudResource) as GameObject);
        hud.transform.parent = transform;
        weaponUiInterface = hud.GetComponent<WeaponUi>();
        weaponUiInterface.setCrossHair(crossHair);
        weaponUiInterface.setName(weaponName);
        weaponUiInterface.setshellIcon(shellIcon);
        weaponUiInterface.setWeaponIcon(weaponIcon);
        weaponUiInterface.setLockOnCrossHair(lockOnIcon);
        reloadShells(0);
    }
    new void Start(){

        // Instantiate UI
        base.Start();
        switch (standardisedRangeSetting){
            case standardisedRange.Short: range = shortRange; break;
            case standardisedRange.Medium: range = mediumRange; break;
            case standardisedRange.Long: range = longRange; break;
            case standardisedRange.Artillery: range = artilleryRange; break;
            case standardisedRange.Infinite: range = InfiniteRange; break;
            default: range = shortRange; break;
        }
        muzzleVelocityBase = muzzleVelocity;
        
        
        mainCamControl = GameObject.FindObjectOfType<CameraLookController>();
        mainCamInteraction = GameObject.FindObjectOfType<CameraInteractionController>();
        if(mainHud == null){
           if(transform.root.GetComponentInChildren<ControlInterface>() != null)mainHud = transform.root.GetComponentInChildren<ControlInterface>().gameObject;
        }
       // request hardpoint settings

       Hardpoint hp = GetComponentInParent<Hardpoint>();
       if(hp!=null) hp.requestSettings(gameObject);
    }

    public virtual void Fire(){

    }

    // used for continuious weapon control to halt fire effects
    public virtual void ceaseFireBeam(){

    }
    public virtual void setLookDir(Vector3 pos, bool instant){

    }
    
    void GetOriginal(){
        originalMats.Clear();
        MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
        foreach(MeshRenderer renderer in renderers){
           // renderer.GetComponent<MeshRenderer>().material.SetColor("mouseOver" ,m_MouseOverColor);
          // Debug.Log("add");
            originalMats.Add(renderer.material);
        }

    }
     void setChildrenMaterials(Material mat){

        MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
        int i = 0;
        foreach(MeshRenderer renderer in renderers){
           // renderer.GetComponent<MeshRenderer>().material.SetColor("mouseOver" ,m_MouseOverColor);
            Material matTemp = mat;
            matTemp.mainTexture = renderer.material.mainTexture;
            renderer.material = matTemp;
            renderer.material.mainTexture = originalMats[i].mainTexture;
            i++;
        }
    }
    
    void restoreChildrenMaterials(){
       MeshRenderer[] renderers = transform.GetComponentsInChildren<MeshRenderer>();
       int i = 0;
        foreach(MeshRenderer renderer in renderers){
           // renderer.GetComponent<MeshRenderer>().material.SetColor("mouseOver" ,m_MouseOverColor);
            
            renderer.material = originalMats[i];
            i++;
        }
    }


    void getShipFlags(){
        Ship myShip = transform.GetComponentInParent<Ship>();
        controllable = myShip.controllable;
        currentlyControlled = myShip.currentlyControlled;
    }
    public void setHighlight(bool set){

        highlighted = set;

    }

    // new highlight stuff
    protected void LateUpdate(){

        if(highlighted == true && !setMaterial){
            setMaterial = true;
            GetOriginal();
            getShipFlags();
            if(currentlyControlled)setChildrenMaterials(mouseOverMaterialControlled);
            else if(controllable) setChildrenMaterials(mouseOverMaterialFriendly);
        }
        else if (highlighted == false && setMaterial){
            setMaterial = false;
            restoreChildrenMaterials();
        }

        highlighted = false;
    }






}

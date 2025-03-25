using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Liikkumisasetukset")]
    public float liikkumisnopeus = 7f;
    public float juoksunopeus = 12f;
    public float hyppyvoima = 7f;
    public float ilmanohjaus = 0.8f;
    
    [Header("Kamera-asetukset")] 
    public float hiiriherkkyys = 2f;
    public float kameraKorkeus = 1.7f;
    public float kameraEtaisyys = 5f;
    public float kameraSileys = 10f;
    
    private Rigidbody rb;
    private Camera mainCamera;
    private Transform kameraKohde;
    private float kameraKulmaX;
    private float kameraKulmaY;
    private bool maassa;
    private Vector3 liikkumisSuunta;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.linearDamping = 1f;
        
        // Kameran alustus
        mainCamera = Camera.main;
        GameObject kameraKohdeObj = new GameObject("KameraKohde");
        kameraKohde = kameraKohdeObj.transform;
        kameraKohde.position = transform.position + Vector3.up * kameraKorkeus;
        
        // Hiiren lukitus
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        KameranLiikutus();
        LiikkumisSyote();
        MaassaTarkistus();
        
        if (maassa && Input.GetKeyDown(KeyCode.Space))
        {
            Hyppaa();
        }
    }

    void FixedUpdate()
    {
        Liiku();
    }

    void KameranLiikutus()
    {
        kameraKulmaX += Input.GetAxis("Mouse X") * hiiriherkkyys;
        kameraKulmaY -= Input.GetAxis("Mouse Y") * hiiriherkkyys;
        kameraKulmaY = Mathf.Clamp(kameraKulmaY, -85f, 85f);

        // Kameran kohteen päivitys
        kameraKohde.position = transform.position + Vector3.up * kameraKorkeus;
        kameraKohde.rotation = Quaternion.Euler(kameraKulmaY, kameraKulmaX, 0);

        // Kameran sijainnin päivitys
        Vector3 haluttuSijainti = kameraKohde.position - kameraKohde.forward * kameraEtaisyys;
        
        // Törmäystarkistus seinien läpi
        RaycastHit hit;
        if (Physics.Linecast(kameraKohde.position, haluttuSijainti, out hit))
        {
            haluttuSijainti = hit.point + hit.normal * 0.2f;
        }

        mainCamera.transform.position = Vector3.Lerp(
            mainCamera.transform.position,
            haluttuSijainti,
            Time.deltaTime * kameraSileys
        );
        mainCamera.transform.LookAt(kameraKohde.position);
    }

    void LiikkumisSyote()
    {
        float vaaka = Input.GetAxisRaw("Horizontal");
        float pysty = Input.GetAxisRaw("Vertical");

        Vector3 eteenpain = mainCamera.transform.forward;
        Vector3 oikealle = mainCamera.transform.right;
        eteenpain.y = 0;
        oikealle.y = 0;
        eteenpain = eteenpain.normalized;
        oikealle = oikealle.normalized;

        liikkumisSuunta = (eteenpain * pysty + oikealle * vaaka).normalized;
    }

    void Liiku()
    {
        float nykyinenNopeus = Input.GetKey(KeyCode.LeftShift) ? juoksunopeus : liikkumisnopeus;
        
        // Maassa liikkuminen
        if (maassa)
        {
            rb.AddForce(liikkumisSuunta * nykyinenNopeus, ForceMode.Force);
        }
        // Ilmassa liikkuminen
        else
        {
            rb.AddForce(liikkumisSuunta * nykyinenNopeus * ilmanohjaus, ForceMode.Force);
        }

        // Nopeuden rajoitus
        Vector3 vaakasuuntainenNopeus = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);
        if (vaakasuuntainenNopeus.magnitude > nykyinenNopeus)
        {
            Vector3 rajoitettuNopeus = vaakasuuntainenNopeus.normalized * nykyinenNopeus;
            rb.linearVelocity = new Vector3(rajoitettuNopeus.x, rb.linearVelocity.y, rajoitettuNopeus.z);
        }
    }

    void Hyppaa()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(Vector3.up * hyppyvoima, ForceMode.Impulse);
        maassa = false;
    }

    void MaassaTarkistus()
    {
        maassa = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}

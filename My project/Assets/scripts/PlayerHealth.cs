using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    public int maxHealth = 100; // Maksimielämä
    private int currentHealth;  // Nykyinen elämä

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Pelaajan elämä: " + currentHealth);
    }

    // Metodi vahinkojen ottamiseen
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Vahinkoa: " + damage + ", nykyinen elämä: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Metodi terveyden palauttamiseen
    public void Heal(int healAmount)
    {
        currentHealth += healAmount;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        Debug.Log("Parannettu, nykyinen elämä: " + currentHealth);
    }

    // Metodi pelaajan kuoleman käsittelyyn
    private void Die()
    {
        Debug.Log("Pelaaja kuoli! Aloitetaan peli alusta.");
        // Ladataan nykyinen taso uudelleen
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
} 
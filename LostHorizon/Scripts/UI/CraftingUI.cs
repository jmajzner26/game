using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace LostHorizon.UI
{
    /// <summary>
    /// UI for the crafting system
    /// </summary>
    public class CraftingUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private GameObject craftingPanel;
        [SerializeField] private Transform recipeContainer;
        [SerializeField] private GameObject recipeButtonPrefab;
        [SerializeField] private Button closeButton;

        [Header("Recipe Display")]
        [SerializeField] private TextMeshProUGUI recipeNameText;
        [SerializeField] private TextMeshProUGUI recipeDescriptionText;
        [SerializeField] private Transform ingredientContainer;
        [SerializeField] private GameObject ingredientSlotPrefab;
        [SerializeField] private Button craftButton;

        private Crafting.CraftingSystem craftingSystem;
        private Player.PlayerInventory playerInventory;
        private Crafting.CraftingStation currentStation;
        private Crafting.CraftingRecipe selectedRecipe;

        private void Start()
        {
            craftingSystem = Crafting.CraftingSystem.Instance;
            playerInventory = FindObjectOfType<Player.PlayerInventory>();

            if (closeButton != null)
            {
                closeButton.onClick.AddListener(CloseCraftingMenu);
            }

            if (craftButton != null)
            {
                craftButton.onClick.AddListener(CraftSelectedRecipe);
            }

            if (craftingPanel != null)
            {
                craftingPanel.SetActive(false);
            }
        }

        private void Update()
        {
            if (craftingPanel != null && craftingPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                CloseCraftingMenu();
            }
        }

        public void OpenCraftingMenu(Crafting.CraftingStation station = null)
        {
            currentStation = station;
            
            if (craftingPanel != null)
            {
                craftingPanel.SetActive(true);
                RefreshRecipeList();
                Time.timeScale = 0f;
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void CloseCraftingMenu()
        {
            if (craftingPanel != null)
            {
                craftingPanel.SetActive(false);
                selectedRecipe = null;
                Time.timeScale = 1f;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

        private void RefreshRecipeList()
        {
            if (recipeContainer == null || craftingSystem == null) return;

            // Clear existing buttons
            foreach (Transform child in recipeContainer)
            {
                Destroy(child.gameObject);
            }

            // Get available recipes
            List<Crafting.CraftingRecipe> recipes;
            if (currentStation != null)
            {
                recipes = craftingSystem.GetAvailableRecipes().FindAll(r => currentStation.CanCraftRecipe(r));
            }
            else
            {
                recipes = craftingSystem.GetAvailableRecipes();
            }

            // Create recipe buttons
            foreach (var recipe in recipes)
            {
                CreateRecipeButton(recipe);
            }
        }

        private void CreateRecipeButton(Crafting.CraftingRecipe recipe)
        {
            if (recipeButtonPrefab == null || recipeContainer == null) return;

            GameObject buttonObj = Instantiate(recipeButtonPrefab, recipeContainer);
            Button button = buttonObj.GetComponent<Button>();

            // Set button text
            TextMeshProUGUI buttonText = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
            {
                buttonText.text = recipe.recipeName;
            }

            // Check if craftable
            bool canCraft = recipe.CanCraft(playerInventory);
            if (button != null)
            {
                button.interactable = canCraft;
                button.onClick.AddListener(() => SelectRecipe(recipe));
            }
        }

        private void SelectRecipe(Crafting.CraftingRecipe recipe)
        {
            selectedRecipe = recipe;
            DisplayRecipeDetails(recipe);
        }

        private void DisplayRecipeDetails(Crafting.CraftingRecipe recipe)
        {
            if (recipeNameText != null)
            {
                recipeNameText.text = recipe.recipeName;
            }

            if (recipeDescriptionText != null)
            {
                recipeDescriptionText.text = recipe.description;
            }

            // Display ingredients
            if (ingredientContainer != null)
            {
                foreach (Transform child in ingredientContainer)
                {
                    Destroy(child.gameObject);
                }

                foreach (var ingredient in recipe.ingredients)
                {
                    GameObject ingredientObj = Instantiate(ingredientSlotPrefab, ingredientContainer);
                    TextMeshProUGUI text = ingredientObj.GetComponentInChildren<TextMeshProUGUI>();
                    if (text != null)
                    {
                        int currentAmount = playerInventory != null ? playerInventory.GetResourceCount(ingredient.resourceType) : 0;
                        bool hasEnough = currentAmount >= ingredient.amount;
                        text.text = $"{ingredient.resourceType}: {currentAmount}/{ingredient.amount}";
                        text.color = hasEnough ? Color.white : Color.red;
                    }
                }
            }

            // Update craft button
            if (craftButton != null)
            {
                craftButton.interactable = recipe.CanCraft(playerInventory);
            }
        }

        private void CraftSelectedRecipe()
        {
            if (selectedRecipe == null || craftingSystem == null) return;

            if (craftingSystem.CraftRecipe(selectedRecipe))
            {
                RefreshRecipeList();
                DisplayRecipeDetails(selectedRecipe);
            }
        }
    }
}


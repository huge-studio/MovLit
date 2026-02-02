Feature: Display Inventory
  As a user playing an Ink story with collectible items
  I want to see my collected inventory items
  So that I can track what I have gathered

  Scenario: Load inventory items on initialization
    Given the inventory module is loaded
    And a filter type is configured in settings
    When the inventory initializes
    Then all items matching the filter type should be displayed

  Scenario: Display inventory with no filter type
    Given the inventory module is loaded
    And no filter type is configured
    When the inventory initializes
    Then the inventory should display an empty list

  Scenario: Refresh inventory when items change
    Given the inventory module is loaded
    And items are displayed
    When the InventoryRefreshed property change event fires
    Then the inventory list should be refreshed with the latest items

  Scenario: Filter items by configured type
    Given the inventory settings have filter type set to "weapon"
    And items of types "weapon" and "potion" are registered
    When the inventory loads
    Then only items of type "weapon" should be displayed

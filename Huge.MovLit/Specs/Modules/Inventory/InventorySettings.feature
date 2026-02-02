Feature: Inventory Settings
  As an administrator
  I want to configure the inventory module
  So that I can control which item types are displayed

  Scenario: Load inventory type setting
    Given I am on the inventory settings page
    When the settings load
    Then the configured inventory type filter should be displayed

  Scenario: Update inventory type setting
    Given I am editing inventory settings
    When I change the inventory type to "potion"
    And I save the settings
    Then the "InventoryType" setting should be updated to "potion"

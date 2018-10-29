pragma solidity ^1.20.0;

import "BasicToken.sol";

contract MyBasicToken is BasicToken {
  uint8 public constant decimals = 18; // solium-disable-line uppercase
  uint256 public constant INITIAL_SUPPLY = 10000 * (10 ** uint256(decimals));

  /**
   * @dev Constructor that gives msg.sender all of existing tokens.
   */
  function MyBasicToken() public {
    totalSupply_ = INITIAL_SUPPLY;
    balances[msg.sender] = INITIAL_SUPPLY;
    Transfer(0x0, msg.sender, INITIAL_SUPPLY);
  }
}
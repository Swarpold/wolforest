using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using UnityEngine.UI;

public class VotePlayerItem : MonoBehaviour
{
    [SerializeField] private Text _playerNameText;
    [SerializeField] private Text _statusText;

    private int _actorNumber;
    public int ActorNumber
    {
        get { return _actorNumber; }
    }
    private Button _voteButton;
    private VotingManager _votingManager;

    private void Awake()
    {
        _voteButton = GetComponentInChildren<Button>();
        _voteButton.onClick.AddListener(OnVotePressed);
    }

    private void OnVotePressed()
    {
        _votingManager.CastVote(_actorNumber);
    }

    public void Initialize(Player player, VotingManager votingManager)
    {
        _actorNumber = player.ActorNumber;
        _playerNameText.text = player.NickName;
        _statusText.text = "Not Decided";
        _votingManager = votingManager;
    }

    public void UpdateStatus(string status)
    {
        _statusText.text = status;

    }

    public void ToggleButton(bool isInteractible)
    {
        _voteButton.interactable = isInteractible;
    }
}

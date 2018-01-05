public class ClientEvent
{
    //Register
    public const string RegisterSuccess = "RegisterSuccess";//注册成功
    public const string RegisterError = "RegisterError";//注册错误,<byte>

    //Login
    public const string LoginSuccess = "LoginSuccess";//登录成功
    public const string LoginError = "LoginError";//登录错误,<byte>

    //Account
    public const string AccountInfo = "AccountInfo";//账户信息,<AccountModel>
    public const string NoAccountInfo = "NoAccountInfo";//账户没有信息
    public const string AccountCreate = "AccountCreate";//创建账户,<bool>
    public const string AccountOnline = "AccountOnline";//账户在线,<AccountModel>

    //Match
    public const string MatchEnterSelect = "MatchEnterSelect";//匹配进入选择

    //MatchSelect
    public const string MatchSelectDestroyBroadcast = "MatchSelectDestroyBroadcast";//匹配选择广播销毁
    public const string MatchSelectEnter = "MatchSelectEnter";//匹配选择进入,<SelectRoomModel>
    public const string MatchSelectRefreshBroadcast = "MatchSelectRefreshBroadcast";//匹配选择广播刷新,<SelectRoomModel>
    public const string MatchSelectSelectFailed = "MatchSelectSelectFailed";//匹配选择选择失败
    public const string MatchSelectReadyBroadcast = "MatchSelectReadyBroadcast";//匹配选择广播准备好,<SelectModel>
    public const string MatchSelectFightBroadcast = "MatchSelectFightBroadcast";//匹配选择广播战斗
    public const string MatchSelectChatBroadcast = "MatchSelectChatBroadcast";//匹配选择广播聊天,<string>

    //Fight
    public const string FightStartBroadcast = "FightStartBroadcast";//战斗广播开始,<FightRoomModel>
    public const string FightMoveBroadcast = "FightMoveBroadcast";//战斗广播移动,<MoveModel>
    public const string FightAttackBroadcast = "FightAttackBroadcast";//战斗广播攻击,<AttackModel>
    public const string FightSkillBroadcast = "FightSkillBroadcast";//战斗广播技能,<SkillAttackModel>
    public const string FightUISkill = "FightUISkill";//战斗UI技能,<int>
    public const string FightDamageBroadcast = "FightDamageBroadcast";//战斗广播伤害,<DamageModel>
    public const string FightDeadBroadcast = "FightDeadBroadcast";//战斗广播死亡,<DeadModel>
    public const string FightRefreshMonsterBroadcast = "FightRefreshMonsterBroadcast";//战斗广播刷新怪物,<FightMonsterModel>
    public const string FightGameOverBroadcast = "FightGameOverBroadcast";//战斗广播游戏结束,<int>
}
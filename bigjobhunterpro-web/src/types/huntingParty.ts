export interface HuntingParty {
  id: string;
  name: string;
  inviteCode: string;
  memberCount: number;
  createdDate: string;
  isCreator: boolean;
}

export interface HuntingPartyMember {
  userId: string;
  displayName: string;
  totalPoints: number;
  joinedDate: string;
  role: string;
}

export interface HuntingPartyDetail {
  id: string;
  name: string;
  inviteCode: string;
  createdDate: string;
  isCreator: boolean;
  members: HuntingPartyMember[];
}

export interface CreateHuntingPartyRequest {
  name: string;
}

export interface JoinHuntingPartyRequest {
  inviteCode: string;
}

export interface LeaderboardEntry {
  rank: number;
  userId: string;
  displayName: string;
  totalPoints: number;
  applicationCount: number;
  isCurrentUser: boolean;
}

export interface RivalInfo {
  displayName: string;
  points: number;
  gap: number;
}

export interface RivalryData {
  currentRank: number;
  totalMembers: number;
  userAhead: RivalInfo | null;
  userBehind: RivalInfo | null;
}

# Points Scoring Table

This table is the source of truth for application points. It is implemented in
`src/Application/Scoring/PointsRules.cs` and used by the points calculation service.

## Status Awards

| Status | Points | When Awarded |
| --- | --- | --- |
| Applied | 1 | On creation |
| Screening | 2 | On status change |
| Interview | 5 | On status change |
| Offer | 50 | On status change |
| Rejected | 5 | On status change |
| Withdrawn | 0 | No points |

## Awarding Rules

- Points are only added when moving to a higher-value status.
- Re-applying the same status does not add points.
- Status downgrades do not remove points.

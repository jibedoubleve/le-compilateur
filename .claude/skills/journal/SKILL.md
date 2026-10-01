---
name: journal
description: Append the current session's learning confusions to the private learning journal (.learning/journal.md) and update the learning profile (.learning/profile.md). Use when the user types /journal or says "journalise", or before /clear, compaction or leaving.
---

# Journal d'apprentissage

Goal: capture how the user learns (confusions, what unblocked them,
teaching mistakes), not the theory itself. Write in French; quotes
stay verbatim.

## Steps

1. Read `.learning/journal.md` and `.learning/profile.md` (create
   them with the same headers if missing; `.learning/` is git-ignored).
2. Find the last session entry. Only journal what happened in the
   current conversation and is not already recorded. If an entry for
   today's topic exists, append to it instead of duplicating it.
3. Append a section `## YYYY-MM-DD — <topic>` with one `### Cn.`
   per confusion, numbering continuing from the previous section of
   the same day, each with exactly these fields:
   - **Citation** — the user's own words that reveal the confusion
   - **Lacune** — the missing distinction, stated precisely
   - **Cause** — apprenant / enseignant / les deux, with one-line
     justification. Be honest about teaching mistakes (ambiguous
     wording, Socratic loops, off-scope answers, unanswered questions).
   - **Déblocage** — what actually worked, or "—"
   - **Statut** — résolu / fragile / ouvert
   Update the status of earlier entries if this session resolved
   or reopened them (note the date).
4. End the section with "Observations transverses" only if a
   cross-cutting pattern appeared.
5. Update `profile.md`: increment occurrence counts with entry
   references, add new hypotheses, and mark a pattern **confirmé**
   at ~3 occurrences. Update "Dernière mise à jour".
6. Report to the user in a few lines: entries added, counts that
   changed, any pattern newly confirmed. Ask them to correct any
   attribution they disagree with.

Do not invent confusions to fill the journal: a session with nothing
notable gets a one-line entry saying so.

---
name: journal
description: Append the current session's learning confusions to the private learning journal (.learning/journal.md) and update the learning profile (.learning/profile.md). Use when the user types /journal or says "journalise", or before /clear, compaction or leaving.
---

# Journal d'apprentissage

Goal: capture how the user learns (confusions, what unblocked them,
teaching mistakes), not the theory itself. Write in the user's
language; if the journal already exists, keep its language and its
field labels. Quotes stay verbatim. The field names below are given
in English: translate them once, then reuse the existing labels.

## Steps

1. Read `.learning/journal.md` and `.learning/profile.md` (create
   them with the same headers if missing; `.learning/` is git-ignored).
2. Find the last session entry. Only journal what happened in the
   current conversation and is not already recorded. If an entry for
   today's topic exists, append to it instead of duplicating it.
3. Append a section `## YYYY-MM-DD — <topic>`. Its first line is
   `**Conditions**: good / average / degraded — <one-word cause>`
   (fatigue, interruptions, noise…), as reported by the user; if they
   said nothing, ask once, otherwise write "not reported". Then one
   `### Cn.` per confusion, numbering continuing from the previous
   section of the same day, each with exactly these fields:
   - **Quote** — the user's own words that reveal the confusion
   - **Gap** — the missing distinction, stated precisely
   - **Cause** — learner / teacher / both, with one-line
     justification. Be honest about teaching mistakes (ambiguous
     wording, Socratic loops, off-scope answers, unanswered questions).
   - **Unblocked by** — what actually worked, or "—"
   - **Status** — resolved / fragile / open
   Update the status of earlier entries if this session resolved
   or reopened them (note the date). A doubt the user voiced on
   purpose and then resolved correctly is not a confusion.
4. End the section with "Cross-cutting observations" only if a
   cross-cutting pattern appeared.
5. Update `profile.md`: increment occurrence counts with entry
   references, add new hypotheses, and mark a pattern **confirmed**
   at ~3 occurrences. Update the "last updated" line.
   Weight by conditions: next to each count, note how many
   occurrences come from degraded sessions (e.g. `5, 3 of them
   degraded`). A pattern whose occurrences come mostly from degraded
   sessions is labelled **context-related**, not a learner weakness,
   until it shows up in good conditions too.
6. Report to the user in a few lines: entries added, counts that
   changed, any pattern newly confirmed. Ask them to correct any
   attribution they disagree with.

Do not invent confusions to fill the journal: a session with nothing
notable gets a one-line entry saying so.

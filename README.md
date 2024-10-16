# Audio Demo (Dynamic SFX)

This is a demo project showcasing one way to have sound effects that change pitch to fit the music. This approach is not neccessarily the optimal version, but it hopefully provides an introduction to some of the main concepts you need to understand.

### A note on approach

This approach, using the in-engine pitch feature to change the pitch of a file, works best with short sounds (e.g. an orchestra hit, perhaps paired with a sword impact) or with chord progressions that include relatively small amounts of movement. A more practical approach would be to create a different version of the sound effect for every chord in the progression in an external program, since this offers more control over the length and envelope of the sound.

### A note on tuning

When determining the ratio to apply to the pitch attribute, there are many possible interpretations of what it means to be "in tune." For example, if you have a sound effect of a C major chord and want it to work over a G major chord, the simplest solution is to set the pitch to 1.5. But unless your track uses just intonation, the value should *technically* be 1.49830707688. Musicians love to argue about this, though usually with fewer numbers involved.
/**
 * This is a TypeGen auto-generated file.
 * Any changes made to this file can be lost when this file is regenerated.
 */

import { SessionState } from "./session-state";
import { EventEntry } from "./event-entry";
import { CarPosition } from "./car-position";
import { FlagDuration } from "./flag-duration";
import { Section } from "./section";
import { Announcement } from "./announcement";
import { CompletedSection } from "./completed-section";
import { EventListSummary } from "./event-list-summary";
import { Event } from "./event";
import { Session } from "./session";
import { CompetitorMetadata } from "./competitor-metadata";
import { ControlLogEntry } from "./control-log-entry";
import { CarControlLogs } from "./car-control-logs";
import { SessionStatePatch } from "./session-state-patch";
import { CarPositionPatch } from "./car-position-patch";
import { VideoStatus } from "./video-status";
import { VideoDestination } from "./video-destination";
import { BroadcasterConfig } from "./broadcaster-config";
import { EventSchedule } from "./event-schedule";
import { EventScheduleEntry } from "./event-schedule-entry";
import { Flags } from "./flags";
import { VideoDestinationType } from "./video-destination-type";
import { VideoSystemType } from "./video-system-type";

export function decodeSessionState(a: unknown[]): SessionState {
    return {
        eventId: a[0] as number,
        eventName: a[1] as string,
        sessionId: a[2] as number,
        sessionName: a[3] as string,
        lapsToGo: a[4] as number,
        timeToGo: a[5] as string,
        localTimeOfDay: a[6] as string,
        runningRaceTime: a[7] as string,
        isPracticeQualifying: a[8] as boolean,
        sessionStartTime: a[9] as Date,
        sessionEndTime: a[10] as Date | null,
        localTimeZoneOffset: a[11] as number,
        isLive: a[12] as boolean,
        eventEntries: (a[13] as unknown[][]).map(decodeEventEntry),
        carPositions: (a[14] as unknown[][]).map(decodeCarPosition),
        currentFlag: a[15] as Flags,
        flagDurations: (a[16] as unknown[][]).map(decodeFlagDuration),
        greenTimeMs: a[17] as number | null,
        greenLaps: a[18] as number | null,
        yellowTimeMs: a[19] as number | null,
        yellowLaps: a[20] as number | null,
        numberOfYellows: a[21] as number | null,
        redTimeMs: a[22] as number | null,
        averageRaceSpeed: a[23] as string,
        leadChanges: a[24] as number | null,
        sections: (a[25] as unknown[][]).map(decodeSection),
        classColors: a[26] as { [key: string]: string; },
        announcements: (a[27] as unknown[][]).map(decodeAnnouncement),
        lastUpdated: a[28] as Date | null,
        classOrder: a[29] as { [key: string]: string; },
        isSimulation: a[30] as boolean,
        isArchived: a[31] as boolean,
        trackTempDegF: a[32] as number | null,
        trackPrecipitationPerc: a[33] as number | null,
    };
}

export function decodeEventEntry(a: unknown[]): EventEntry {
    return {
        number: a[0] as string,
        name: a[1] as string,
        team: a[2] as string,
        class: a[3] as string,
    };
}

export function decodeCarPosition(a: unknown[]): CarPosition {
    return {
        invalidPosition: -999,
        eventId: a[0] as string,
        sessionId: a[1] as string,
        number: a[2] as string,
        transponderId: a[3] as number,
        class: a[4] as string,
        bestTime: a[5] as string,
        bestLap: a[6] as number,
        isBestTime: a[7] as boolean,
        isBestTimeClass: a[8] as boolean,
        inClassGap: a[9] as string,
        inClassDifference: a[10] as string,
        overallGap: a[11] as string,
        overallDifference: a[12] as string,
        totalTime: a[13] as string,
        lastLapTime: a[14] as string,
        lastLapCompleted: a[15] as number,
        pitStopCount: a[16] as number | null,
        lastLapPitted: a[17] as number | null,
        lapsLedOverall: a[18] as number | null,
        lapsLedInClass: a[19] as number | null,
        overallPosition: a[20] as number,
        classPosition: a[21] as number,
        overallStartingPosition: a[22] as number,
        overallPositionsGained: a[23] as number,
        inClassStartingPosition: a[24] as number,
        inClassPositionsGained: a[25] as number,
        isOverallMostPositionsGained: a[26] as boolean,
        isClassMostPositionsGained: a[27] as boolean,
        penalityLaps: a[28] as number,
        penalityWarnings: a[29] as number,
        blackFlags: a[30] as number,
        isEnteredPit: a[31] as boolean,
        isPitStartFinish: a[32] as boolean,
        isExitedPit: a[33] as boolean,
        isInPit: a[34] as boolean,
        lapIncludedPit: a[35] as boolean,
        lastLoopName: a[36] as string,
        isStale: a[37] as boolean,
        trackFlag: a[38] as Flags,
        localFlag: a[39] as Flags,
        lapHadLocalFlag: a[40] as boolean | null,
        completedSections: (a[41] as unknown[][]).map(decodeCompletedSection),
        projectedLapTimeMs: a[42] as number,
        lapStartTime: a[43] as string,
        driverName: a[44] as string,
        driverId: a[45] as string,
        inCarVideo: a[46] != null ? decodeVideoStatus(a[46] as unknown[]) : null,
        latitude: a[47] as number | null,
        longitude: a[48] as number | null,
        currentStatus: a[49] as string,
        impactWarning: a[50] as boolean,
        inClassFastestAveragePace: a[51] as boolean,
        trackTempDegF: a[52] as number | null,
        trackPrecipitationPerc: a[53] as number | null,
    };
}

export function decodeFlagDuration(a: unknown[]): FlagDuration {
    return {
        flag: a[0] as Flags,
        startTime: a[1] as Date,
        endTime: a[2] as Date | null,
    };
}

export function decodeSection(a: unknown[]): Section {
    return {
        name: a[0] as string,
        lengthInches: a[1] as number,
        startLabel: a[2] as string,
        endLabel: a[3] as string,
    };
}

export function decodeAnnouncement(a: unknown[]): Announcement {
    return {
        timestamp: a[0] as Date,
        priority: a[1] as string,
        text: a[2] as string,
    };
}

export function decodeCompletedSection(a: unknown[]): CompletedSection {
    return {
        number: a[0] as string,
        sectionId: a[1] as string,
        elapsedTimeMs: a[2] as number,
        lastSectionTimeMs: a[3] as number,
        lastLap: a[4] as number,
    };
}

export function decodeEventListSummary(a: unknown[]): EventListSummary {
    return {
        id: a[0] as number,
        organizationId: a[1] as number,
        organizationName: a[2] as string,
        eventName: a[3] as string,
        eventDate: a[4] as string,
        isLive: a[5] as boolean,
        trackName: a[6] as string,
        schedule: a[7] != null ? decodeEventSchedule(a[7] as unknown[]) : null,
        isSimulation: a[8] as boolean,
        isArchived: a[9] as boolean,
    };
}

export function decodeEvent(a: unknown[]): Event {
    return {
        eventId: a[0] as number,
        eventName: a[1] as string,
        eventDate: a[2] as string,
        eventUrl: a[3] as string,
        sessions: (a[4] as unknown[][]).map(decodeSession),
        organizationName: a[5] as string,
        organizationWebsite: a[6] as string,
        organizationLogo: a[7] as number[],
        schedule: a[8] != null ? decodeEventSchedule(a[8] as unknown[]) : null,
        trackName: a[9] as string,
        courseConfiguration: a[10] as string,
        distance: a[11] as string,
        broadcast: a[12] != null ? decodeBroadcasterConfig(a[12] as unknown[]) : null,
        hasControlLog: a[13] as boolean,
        isLive: a[14] as boolean,
        isSimulation: a[15] as boolean,
        isArchived: a[16] as boolean,
        organizationId: a[17] as number,
    };
}

export function decodeSession(a: unknown[]): Session {
    return {
        id: a[0] as number,
        eventId: a[1] as number,
        name: a[2] as string,
        startTime: a[3] as Date,
        endTime: a[4] as Date | null,
        localTimeZoneOffset: a[5] as number,
        lastUpdated: a[6] as Date | null,
        isLive: a[7] as boolean,
        isPracticeQualifying: a[8] as boolean,
    };
}

export function decodeCompetitorMetadata(a: unknown[]): CompetitorMetadata {
    return {
        eventId: a[0] as number,
        carNumber: a[1] as string,
        lastUpdated: a[2] as Date,
        transponder: a[3] as number,
        transponder2: a[4] as number,
        class: a[5] as string,
        firstName: a[6] as string,
        lastName: a[7] as string,
        nationState: a[8] as string,
        sponsor: a[9] as string,
        make: a[10] as string,
        hometown: a[11] as string,
        club: a[12] as string,
        modelEngine: a[13] as string,
        tires: a[14] as string,
        email: a[15] as string,
    };
}

export function decodeControlLogEntry(a: unknown[]): ControlLogEntry {
    return {
        orderId: a[0] as number,
        timestamp: a[1] as Date,
        corner: a[2] as string,
        car1: a[3] as string,
        isCar1Highlighted: a[4] as boolean,
        car2: a[5] as string,
        isCar2Highlighted: a[6] as boolean,
        note: a[7] as string,
        status: a[8] as string,
        penaltyAction: a[9] as string,
        otherNotes: a[10] as string,
    };
}

export function decodeCarControlLogs(a: unknown[]): CarControlLogs {
    return {
        carNumber: a[0] as string,
        controlLogEntries: (a[1] as unknown[][]).map(decodeControlLogEntry),
    };
}

export function decodeSessionStatePatch(a: unknown[]): SessionStatePatch {
    return {
        eventId: a[0] as number | null,
        eventName: a[1] as string,
        sessionId: a[2] as number | null,
        sessionName: a[3] as string,
        lapsToGo: a[4] as number | null,
        timeToGo: a[5] as string,
        localTimeOfDay: a[6] as string,
        runningRaceTime: a[7] as string,
        isPracticeQualifying: a[8] as boolean | null,
        sessionStartTime: a[9] as Date | null,
        sessionEndTime: a[10] as Date | null,
        localTimeZoneOffset: a[11] as number | null,
        isLive: a[12] as boolean | null,
        eventEntries: a[13] != null ? (a[13] as unknown[][]).map(decodeEventEntry) : null,
        carPositions: a[14] != null ? (a[14] as unknown[][]).map(decodeCarPosition) : null,
        currentFlag: a[15] as Flags | null,
        flagDurations: a[16] != null ? (a[16] as unknown[][]).map(decodeFlagDuration) : null,
        greenTimeMs: a[17] as number | null,
        greenLaps: a[18] as number | null,
        yellowTimeMs: a[19] as number | null,
        yellowLaps: a[20] as number | null,
        numberOfYellows: a[21] as number | null,
        redTimeMs: a[22] as number | null,
        averageRaceSpeed: a[23] as string,
        leadChanges: a[24] as number | null,
        sections: a[25] != null ? (a[25] as unknown[][]).map(decodeSection) : null,
        classColors: a[26] as { [key: string]: string; },
        announcements: a[27] != null ? (a[27] as unknown[][]).map(decodeAnnouncement) : null,
        lastUpdated: a[28] as Date | null,
        classOrder: a[29] as { [key: string]: string; },
        isSimulation: a[30] as boolean | null,
        isArchived: a[31] as boolean | null,
        trackTempDegF: a[32] as number | null,
        trackPrecipitationPerc: a[33] as number | null,
    };
}

export function decodeCarPositionPatch(a: unknown[]): CarPositionPatch {
    return {
        eventId: a[0] as string,
        sessionId: a[1] as string,
        number: a[2] as string,
        transponderId: a[3] as number | null,
        class: a[4] as string,
        bestTime: a[5] as string,
        bestLap: a[6] as number | null,
        isBestTime: a[7] as boolean | null,
        isBestTimeClass: a[8] as boolean | null,
        inClassGap: a[9] as string,
        inClassDifference: a[10] as string,
        overallGap: a[11] as string,
        overallDifference: a[12] as string,
        totalTime: a[13] as string,
        lastLapTime: a[14] as string,
        lastLapCompleted: a[15] as number | null,
        pitStopCount: a[16] as number | null,
        lastLapPitted: a[17] as number | null,
        lapsLedOverall: a[18] as number | null,
        lapsLedInClass: a[19] as number | null,
        overallPosition: a[20] as number | null,
        classPosition: a[21] as number | null,
        overallStartingPosition: a[22] as number | null,
        overallPositionsGained: a[23] as number | null,
        inClassStartingPosition: a[24] as number | null,
        inClassPositionsGained: a[25] as number | null,
        isOverallMostPositionsGained: a[26] as boolean | null,
        isClassMostPositionsGained: a[27] as boolean | null,
        penalityLaps: a[28] as number | null,
        penalityWarnings: a[29] as number | null,
        blackFlags: a[30] as number | null,
        isEnteredPit: a[31] as boolean | null,
        isPitStartFinish: a[32] as boolean | null,
        isExitedPit: a[33] as boolean | null,
        isInPit: a[34] as boolean | null,
        lapIncludedPit: a[35] as boolean | null,
        lastLoopName: a[36] as string,
        isStale: a[37] as boolean | null,
        trackFlag: a[38] as Flags | null,
        localFlag: a[39] as Flags | null,
        lapHadLocalFlag: a[40] as boolean | null,
        completedSections: a[41] != null ? (a[41] as unknown[][]).map(decodeCompletedSection) : null,
        projectedLapTimeMs: a[42] as number | null,
        lapStartTime: a[43] as string | null,
        driverName: a[44] as string,
        driverId: a[45] as string,
        inCarVideo: a[46] != null ? decodeVideoStatus(a[46] as unknown[]) : null,
        latitude: a[47] as number | null,
        longitude: a[48] as number | null,
        currentStatus: a[49] as string,
        impactWarning: a[50] as boolean | null,
        inClassFastestAveragePace: a[51] as boolean | null,
        trackTempDegF: a[52] as number | null,
        trackPrecipitationPerc: a[53] as number | null,
    };
}

export function decodeVideoStatus(a: unknown[]): VideoStatus {
    return {
        videoSystemType: a[0] as VideoSystemType,
        videoDestination: decodeVideoDestination(a[1] as unknown[]),
    };
}

export function decodeVideoDestination(a: unknown[]): VideoDestination {
    return {
        type: a[0] as VideoDestinationType,
        url: a[1] as string,
        hostName: a[2] as string,
        port: a[3] as number,
        parameters: a[4] as string,
    };
}

export function decodeBroadcasterConfig(a: unknown[]): BroadcasterConfig {
    return {
        companyName: a[0] as string,
        url: a[1] as string,
    };
}

export function decodeEventSchedule(a: unknown[]): EventSchedule {
    return {
        name: a[0] as string,
        entries: (a[1] as unknown[][]).map(decodeEventScheduleEntry),
    };
}

export function decodeEventScheduleEntry(a: unknown[]): EventScheduleEntry {
    return {
        dayOfEvent: a[0] as Date,
        startTime: a[1] as Date,
        endTime: a[2] as Date,
        name: a[3] as string,
    };
}


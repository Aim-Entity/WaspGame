import { AnimatePresence, motion } from "motion/react";
import { Wasp } from "@/components/Wasp";
import { useGame } from "@/hooks/useGame";
import type { WaspType } from "@/types";

const ROWS: [WaspType, string][] = [
    ["Queen", "Queen"],
    ["Drone", "Drones"],
    ["Worker", "Workers"],
];

export default function App() {
    const { game, lastHit, now, error, zap, reset } = useGame();

    if (!game) {
        return (
            <main className="grid min-h-svh place-items-center text-stone-500">
                {error ?? "Loading…"}
            </main>
        );
    }

    const awake = game.wasps.filter((w) => !w.isKnocked).length;
    const queenDown = game.wasps.some((w) => w.type === "Queen" && w.isKnocked);

    return (
        <main className="mx-auto flex min-h-svh max-w-3xl flex-col items-center justify-center gap-8 px-4 py-12">
            <header className="text-center">
                <h1 className="text-3xl font-semibold tracking-tight">
                    Wasp Nest
                </h1>
                <p className="mt-1 text-sm text-stone-500">
                    {awake} of {game.wasps.length} wasps buzzing
                </p>
            </header>

            <section className="relative flex w-full flex-col gap-8 rounded-3xl bg-white p-8 shadow-sm">
                {ROWS.map(([type, label]) => (
                    <div
                        key={type}
                        className="flex flex-col items-center gap-3"
                    >
                        <h2 className="text-xs uppercase tracking-widest text-stone-400">
                            {label}
                        </h2>
                        <div className="flex flex-wrap justify-center gap-6">
                            {game.wasps
                                .filter((w) => w.type === type)
                                .map((w) => (
                                    <Wasp
                                        key={w.id}
                                        wasp={w}
                                        now={now}
                                        knockoutSeconds={game.knockoutSeconds}
                                        hitKey={
                                            lastHit?.waspId === w.id
                                                ? lastHit.key
                                                : undefined
                                        }
                                    />
                                ))}
                        </div>
                    </div>
                ))}

                <AnimatePresence>
                    {game.isGameDone && (
                        <motion.div
                            className="absolute inset-0 grid place-items-center rounded-3xl bg-white/70 backdrop-blur-sm"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1, transition: { delay: 0.5 } }}
                            exit={{ opacity: 0 }}
                        >
                            <div className="text-center">
                                <p className="text-2xl font-semibold">
                                    Nest cleared
                                </p>
                                <p className="mt-1 text-sm text-stone-500">
                                    {queenDown
                                        ? "The queen has been knocked out."
                                        : "Every wasp is knocked out."}
                                </p>
                            </div>
                        </motion.div>
                    )}
                </AnimatePresence>
            </section>

            <div className="flex gap-3">
                <motion.button
                    whileTap={{ scale: 0.94 }}
                    onClick={zap}
                    disabled={game.isGameDone}
                    className="rounded-full bg-stone-900 px-8 py-3 font-medium text-white disabled:opacity-40 cursor-pointer"
                >
                    Zap
                </motion.button>
                <button
                    onClick={reset}
                    className="rounded-full px-6 py-3 text-stone-600 hover:bg-stone-200 cursor-pointer"
                >
                    Reset
                </button>
            </div>

            {error && <p className="text-sm text-red-600">{error}</p>}
        </main>
    );
}

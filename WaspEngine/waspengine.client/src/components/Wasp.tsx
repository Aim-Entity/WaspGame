import { motion } from "motion/react";
import type { Wasp as WaspModel } from "@/types";

interface WaspProps {
    wasp: WaspModel;
    now: number;
    knockoutSeconds: number;
    hitKey?: number;
}

const SIZES = { Queen: "w-24", Drone: "w-14", Worker: "w-16" };

export function Wasp({ wasp, now, knockoutSeconds, hitKey }: WaspProps) {
    const { isKnocked } = wasp;
    const secondsLeft = wasp.knockedUntil
        ? Math.min(
              knockoutSeconds,
              Math.max(
                  0,
                  Math.ceil((Date.parse(wasp.knockedUntil) - now) / 1000),
              ),
          )
        : 0;

    return (
        <div className="relative flex flex-col items-center gap-2">
            <motion.div
                animate={
                    isKnocked
                        ? {
                              y: 14,
                              rotate: 180,
                              opacity: 0.35,
                              filter: "grayscale(1)",
                          }
                        : {
                              y: [0, -6, 0],
                              rotate: 0,
                              opacity: 1,
                              filter: "grayscale(0)",
                          }
                }
                transition={{
                    default: { duration: 0.5 },
                    y: isKnocked
                        ? { duration: 0.5 }
                        : {
                              duration: 1.8 + (wasp.id % 4) * 0.3,
                              repeat: Infinity,
                              ease: "easeInOut",
                          },
                }}
            >
                <motion.div
                    key={hitKey}
                    animate={
                        hitKey
                            ? {
                                  x: [0, -8, 8, -5, 5, 0],
                                  filter: ["brightness(2)", "brightness(1)"],
                              }
                            : undefined
                    }
                    transition={{ duration: 0.4 }}
                >
                    <WaspIcon
                        className={SIZES[wasp.type]}
                        queen={wasp.type === "Queen"}
                        flapping={!isKnocked}
                    />
                </motion.div>
            </motion.div>

            {hitKey && (
                <motion.span
                    key={hitKey}
                    className="pointer-events-none absolute -top-2"
                    initial={{ opacity: 1, y: 0, scale: 1.2 }}
                    animate={{ opacity: 0, y: -24, scale: 0.8 }}
                    transition={{ duration: 0.8 }}
                >
                    ⚡
                </motion.span>
            )}

            <div className="h-1 w-14 overflow-hidden rounded-full bg-stone-200">
                <motion.div
                    className={`h-full ${wasp.type === "Queen" ? "bg-amber-500" : "bg-yellow-400"}`}
                    animate={{
                        width: `${(wasp.energy / wasp.maxEnergy) * 100}%`,
                    }}
                />
            </div>
            <span className="text-xs tabular-nums text-stone-400">
                {isKnocked ? `${secondsLeft}s` : wasp.energy}
            </span>
        </div>
    );
}

function WaspIcon({
    className,
    queen,
    flapping,
}: {
    className: string;
    queen: boolean;
    flapping: boolean;
}) {
    const dark = "#1c1917";
    return (
        <svg viewBox="0 0 64 48" className={className}>
            <motion.g
                style={{ originY: 1 }}
                animate={{ scaleY: flapping ? [1, 0.35, 1] : 1 }}
                transition={{ duration: 0.12, repeat: flapping ? Infinity : 0 }}
            >
                <ellipse
                    cx="24"
                    cy="13"
                    rx="11"
                    ry="7"
                    fill="#94a3b8"
                    fillOpacity="0.35"
                />
                <ellipse
                    cx="36"
                    cy="12"
                    rx="9"
                    ry="6"
                    fill="#94a3b8"
                    fillOpacity="0.35"
                />
            </motion.g>
            <polygon points="9,28 1,30 9,32" fill={dark} />
            <ellipse
                cx="28"
                cy="30"
                rx="20"
                ry="11"
                fill={queen ? "#f59e0b" : "#facc15"}
            />
            <ellipse cx="20" cy="30" rx="2.5" ry="10" fill={dark} />
            <ellipse cx="30" cy="30" rx="2.5" ry="11" fill={dark} />
            <circle cx="50" cy="28" r="7" fill={dark} />
            <circle cx="53" cy="26" r="1.8" fill="#fef3c7" />
            <path
                d="M52 22 Q56 13 61 14"
                stroke={dark}
                strokeWidth="1.5"
                fill="none"
            />
        </svg>
    );
}

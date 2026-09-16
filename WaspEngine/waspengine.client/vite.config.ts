import { fileURLToPath, URL } from "node:url";

import { defineConfig, loadEnv } from "vite";
import plugin from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import fs from "fs";
import path from "path";
import child_process from "child_process";

const configDir = fileURLToPath(new URL(".", import.meta.url));

// https://vitejs.dev/config/
export default defineConfig(({ mode }) => {
    const env = { ...loadEnv(mode, configDir, ""), ...process.env };

    const baseFolder =
        env.APPDATA !== undefined && env.APPDATA !== ""
            ? `${env.APPDATA}/ASP.NET/https`
            : `${env.HOME}/.aspnet/https`;

    const certificateName = "waspengine.client";
    const certFilePath = path.join(baseFolder, `${certificateName}.pem`);
    const keyFilePath = path.join(baseFolder, `${certificateName}.key`);

    if (!fs.existsSync(baseFolder)) {
        fs.mkdirSync(baseFolder, { recursive: true });
    }

    if (!fs.existsSync(certFilePath) || !fs.existsSync(keyFilePath)) {
        if (
            0 !==
            child_process.spawnSync(
                "dotnet",
                [
                    "dev-certs",
                    "https",
                    "--export-path",
                    certFilePath,
                    "--format",
                    "Pem",
                    "--no-password",
                ],
                { stdio: "inherit" },
            ).status
        ) {
            throw new Error("Could not create certificate.");
        }
    }

    const target = env.ASPNETCORE_HTTPS_PORT
        ? `https://localhost:${env.ASPNETCORE_HTTPS_PORT}`
        : env.VITE_API_ASPNETCORE_URLS
          ? env.VITE_API_ASPNETCORE_URLS.split(";")[0]
          : "https://localhost:7011";

    return {
        plugins: [plugin(), tailwindcss()],
        resolve: {
            alias: {
                "@": fileURLToPath(new URL("./src", import.meta.url)),
            },
        },
        server: {
            proxy: {
                "^/api": {
                    target,
                    secure: false,
                },
            },
            port: parseInt(env.VITE_API_DEV_SERVER_PORT || "52144"),
            https: {
                key: fs.readFileSync(keyFilePath),
                cert: fs.readFileSync(certFilePath),
            },
        },
    };
});

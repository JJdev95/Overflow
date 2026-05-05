'use server';

import { auth } from "@/auth";
import { fetchClient } from "@/lib/fetchClient";

export async function testAuth() {
    return fetchClient<string>(`/test/auth`, 'GET', { cache: 'no-cache' });
}

export async function getCurrentUser() {
    const session = await auth();

    return session?.user ?? null;
}
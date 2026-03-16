import { getTags } from "@/lib/actions/tag-actions";
import TagHeader from "./TagHeader";
import TagCard from "./TagCard";


export default async function TagsPage() {
    const { data: tags, error } = await getTags();

    if (!tags || error) throw new Error(error?.message);

    return (
        <div className='w-full px-6'>
            <TagHeader />
            <div className='grid grid-cols-3 gap-4'>
                {tags.map(tag => (
                    <TagCard key={tag.id} tag={tag} />
                ))}
            </div>
        </div>
    )
}